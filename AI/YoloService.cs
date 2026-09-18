using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BelgeArsivlemeSistemi.AI
{
    public class YoloService : IDisposable
    {
        private readonly InferenceSession session;
        private const int InputSize = 640;

        public YoloService()
        {
            string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AI", "best.onnx");

            if (!File.Exists(modelPath))
                throw new FileNotFoundException("best.onnx bulunamadı.", modelPath);

            session = new InferenceSession(modelPath);
        }

        public Rect? BelgeTespitEt(Mat kaynak, float confidenceThreshold = 0.30f)
        {
            int originalWidth = kaynak.Width;
            int originalHeight = kaynak.Height;

            Mat resized = new Mat();
            Cv2.Resize(kaynak, resized, new OpenCvSharp.Size(InputSize, InputSize));

            Mat rgb = new Mat();
            Cv2.CvtColor(resized, rgb, ColorConversionCodes.BGR2RGB);

            var inputTensor = new DenseTensor<float>(new[] { 1, 3, InputSize, InputSize });

            for (int y = 0; y < InputSize; y++)
            {
                for (int x = 0; x < InputSize; x++)
                {
                    Vec3b pixel = rgb.At<Vec3b>(y, x);

                    inputTensor[0, 0, y, x] = pixel.Item0 / 255.0f;
                    inputTensor[0, 1, y, x] = pixel.Item1 / 255.0f;
                    inputTensor[0, 2, y, x] = pixel.Item2 / 255.0f;
                }
            }

            string inputName = session.InputMetadata.Keys.First();

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
            };

            using var results = session.Run(inputs);

            var output = results.First().AsTensor<float>();
            var dims = output.Dimensions.ToArray();

            int predictionCount = dims[2];

            float bestConf = 0;
            Rect? bestBox = null;

            for (int i = 0; i < predictionCount; i++)
            {
                float xCenter = output[0, 0, i];
                float yCenter = output[0, 1, i];
                float width = output[0, 2, i];
                float height = output[0, 3, i];
                float conf = output[0, 4, i];

                if (conf < confidenceThreshold)
                    continue;

                if (conf > bestConf)
                {
                    bestConf = conf;

                    float x1 = xCenter - width / 2;
                    float y1 = yCenter - height / 2;

                    int realX = (int)(x1 * originalWidth / InputSize);
                    int realY = (int)(y1 * originalHeight / InputSize);
                    int realW = (int)(width * originalWidth / InputSize);
                    int realH = (int)(height * originalHeight / InputSize);

                    bestBox = new Rect(realX, realY, realW, realH);
                }
            }

            return bestBox;
        }

        public List<Rect> BelgeleriTespitEt(Mat kaynak, float confidenceThreshold = 0.30f)
        {
            List<(Rect rect, float conf)> kutular = new();

            int originalWidth = kaynak.Width;
            int originalHeight = kaynak.Height;

            Mat resized = new Mat();
            Cv2.Resize(kaynak, resized, new OpenCvSharp.Size(InputSize, InputSize));

            Mat rgb = new Mat();
            Cv2.CvtColor(resized, rgb, ColorConversionCodes.BGR2RGB);

            var inputTensor = new DenseTensor<float>(new[] { 1, 3, InputSize, InputSize });

            for (int y = 0; y < InputSize; y++)
            {
                for (int x = 0; x < InputSize; x++)
                {
                    Vec3b pixel = rgb.At<Vec3b>(y, x);

                    inputTensor[0, 0, y, x] = pixel.Item0 / 255.0f;
                    inputTensor[0, 1, y, x] = pixel.Item1 / 255.0f;
                    inputTensor[0, 2, y, x] = pixel.Item2 / 255.0f;
                }
            }

            string inputName = session.InputMetadata.Keys.First();

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
            };

            using var results = session.Run(inputs);

            var output = results.First().AsTensor<float>();
            int predictionCount = output.Dimensions[2];

            for (int i = 0; i < predictionCount; i++)
            {
                
                float xCenter = output[0, 0, i];
                float yCenter = output[0, 1, i];
                float width = output[0, 2, i];
                float height = output[0, 3, i];

               
                float conf = output[0, 4, i];

                if (conf < confidenceThreshold)
                    continue;

                float x1 = xCenter - width / 2;
                float y1 = yCenter - height / 2;

                int realX = (int)(x1 * originalWidth / InputSize);
                int realY = (int)(y1 * originalHeight / InputSize);
                int realW = (int)(width * originalWidth / InputSize);
                int realH = (int)(height * originalHeight / InputSize);

                Rect rect = new Rect(realX, realY, realW, realH);

                rect.X = Math.Max(0, rect.X);
                rect.Y = Math.Max(0, rect.Y);
                rect.Width = Math.Min(rect.Width, originalWidth - rect.X);
                rect.Height = Math.Min(rect.Height, originalHeight - rect.Y);

                kutular.Add((rect, conf));
            }

            double resimAlani =
     (double)originalWidth * originalHeight;

           
            var alanFiltresindenGecenler = kutular
                .Where(x =>
                {
                    double kutuAlani =
                        (double)x.rect.Width * x.rect.Height;

                    double alanOrani =
                        kutuAlani / resimAlani;

                    return alanOrani < 0.95;
                })
                .ToList();

            /*
             * Aynı belge için üretilen tekrar kutuları
             * standart NMS mantığıyla azaltıyoruz.
             */
            var nmsSonucu = NonMaximumSuppression(
                alanFiltresindenGecenler,
                0.45);

           
            return nmsSonucu
                .OrderBy(x => x.rect.X)
                .ThenBy(x => x.rect.Y)
                .Select(x => x.rect)
                .ToList();
        }

        public List<(Rect Kutu, float Guven)> HamBelgeleriTespitEt(
    Mat kaynak,
    float confidenceThreshold = 0.05f)
        {
            var kutular = new List<(Rect Kutu, float Guven)>();

            if (kaynak == null || kaynak.Empty())
                return kutular;

            int originalWidth = kaynak.Width;
            int originalHeight = kaynak.Height;

            using Mat resized = new Mat();

            Cv2.Resize(
                kaynak,
                resized,
                new OpenCvSharp.Size(InputSize, InputSize));

            using Mat rgb = new Mat();

            Cv2.CvtColor(
                resized,
                rgb,
                ColorConversionCodes.BGR2RGB);

            var inputTensor =
                new DenseTensor<float>(
                    new[] { 1, 3, InputSize, InputSize });

            for (int y = 0; y < InputSize; y++)
            {
                for (int x = 0; x < InputSize; x++)
                {
                    Vec3b pixel = rgb.At<Vec3b>(y, x);

                    inputTensor[0, 0, y, x] =
                        pixel.Item0 / 255.0f;

                    inputTensor[0, 1, y, x] =
                        pixel.Item1 / 255.0f;

                    inputTensor[0, 2, y, x] =
                        pixel.Item2 / 255.0f;
                }
            }

            string inputName =
                session.InputMetadata.Keys.First();

            var inputs = new List<NamedOnnxValue>
    {
        NamedOnnxValue.CreateFromTensor(
            inputName,
            inputTensor)
    };

            using var results = session.Run(inputs);

            var output =
                results.First().AsTensor<float>();

            int predictionCount =
                output.Dimensions[2];

            for (int i = 0; i < predictionCount; i++)
            {
                float xCenter = output[0, 0, i];
                float yCenter = output[0, 1, i];
                float width = output[0, 2, i];
                float height = output[0, 3, i];

               
                float confidence = output[0, 4, i];

                if (confidence < confidenceThreshold)
                    continue;

                float x1 = xCenter - width / 2.0f;
                float y1 = yCenter - height / 2.0f;

                int realX =
                    (int)(x1 * originalWidth / InputSize);

                int realY =
                    (int)(y1 * originalHeight / InputSize);

                int realWidth =
                    (int)(width * originalWidth / InputSize);

                int realHeight =
                    (int)(height * originalHeight / InputSize);

                realX = Math.Clamp(
                    realX,
                    0,
                    originalWidth - 1);

                realY = Math.Clamp(
                    realY,
                    0,
                    originalHeight - 1);

                realWidth = Math.Clamp(
                    realWidth,
                    1,
                    originalWidth - realX);

                realHeight = Math.Clamp(
                    realHeight,
                    1,
                    originalHeight - realY);

                kutular.Add((
                    new Rect(
                        realX,
                        realY,
                        realWidth,
                        realHeight),
                    confidence));
            }

            return kutular
                .OrderByDescending(x => x.Guven)
                .ToList();
        }

        private double KapsanmaOrani(Rect kucuk, Rect buyuk)
        {
            int x1 = Math.Max(kucuk.Left, buyuk.Left);
            int y1 = Math.Max(kucuk.Top, buyuk.Top);
            int x2 = Math.Min(kucuk.Right, buyuk.Right);
            int y2 = Math.Min(kucuk.Bottom, buyuk.Bottom);

            int genislik = Math.Max(0, x2 - x1);
            int yukseklik = Math.Max(0, y2 - y1);

            double kesisim = genislik * yukseklik;
            double kucukAlan = kucuk.Width * kucuk.Height;

            return kucukAlan <= 0 ? 0 : kesisim / kucukAlan;
        }

        private List<(Rect rect, float conf)> NonMaximumSuppression(
    List<(Rect rect, float conf)> kutular,
    double iouThreshold)
        {
            var kalanKutular = kutular
                .OrderByDescending(x => x.conf)
                .ToList();

            var secilenKutular =
                new List<(Rect rect, float conf)>();

            while (kalanKutular.Count > 0)
            {
                var enGuvenliKutu =
                    kalanKutular[0];

                secilenKutular.Add(enGuvenliKutu);

                kalanKutular.RemoveAt(0);

                kalanKutular = kalanKutular
                    .Where(aday =>
                        IoU(
                            enGuvenliKutu.rect,
                            aday.rect) < iouThreshold)
                    .ToList();
            }

            return secilenKutular;
        }
        private double IoU(Rect a, Rect b)
        {
            int x1 = Math.Max(a.Left, b.Left);
            int y1 = Math.Max(a.Top, b.Top);
            int x2 = Math.Min(a.Right, b.Right);
            int y2 = Math.Min(a.Bottom, b.Bottom);

            int interWidth = Math.Max(0, x2 - x1);
            int interHeight = Math.Max(0, y2 - y1);

            double intersection = interWidth * interHeight;
            double union = a.Width * a.Height + b.Width * b.Height - intersection;

            if (union <= 0)
                return 0;

            return intersection / union;
        }

        public string TestModel()
        {
            var input = session.InputMetadata.First();
            var output = session.OutputMetadata.First();

            return $"Model yüklendi.\nInput: {input.Key}\nOutput: {output.Key}";
        }

        public void Dispose()
        {
            session.Dispose();
        }
    }
}