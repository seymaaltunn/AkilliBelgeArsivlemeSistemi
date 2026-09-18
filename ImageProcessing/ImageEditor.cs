using OpenCvSharp;

namespace BelgeArsivlemeSistemi.ImageProcessing
{
    public class ImageEditor
    {
        // Gri Ton
        public Mat ToGray(Mat kaynak)
        {
            if (kaynak.Channels() == 1)
                return kaynak.Clone();

            Mat sonuc = new Mat();

            Cv2.CvtColor(
                kaynak,
                sonuc,
                ColorConversionCodes.BGR2GRAY);

            return sonuc;
        }

        // Kontrast Artır
        public Mat IncreaseContrast(Mat kaynak)
        {
            Mat sonuc = new Mat();

            kaynak.ConvertTo(
                sonuc,
                -1,
                1.5,
                0);

            return sonuc;
        }

        // Gürültü Azalt
        public Mat RemoveNoise(Mat kaynak)
        {
            Mat sonuc = new Mat();

            Cv2.GaussianBlur(
                kaynak,
                sonuc,
                new OpenCvSharp.Size(5, 5),
                0);

            return sonuc;
        }

        // Keskinleştir
        public Mat Sharpen(Mat kaynak)
        {
            float[] kernelData =
            {
                 0,-1,0,
                -1, 5,-1,
                 0,-1,0
            };

            Mat kernel = Mat.FromPixelData(
                3,
                3,
                MatType.CV_32FC1,
                kernelData);

            Mat sonuc = new Mat();

            Cv2.Filter2D(
                kaynak,
                sonuc,
                -1,
                kernel);

            return sonuc;
        }
        public Mat ImproveTextReadability(Mat kaynak)
        {
            Mat gri = ToGray(kaynak);

            // 1. Yerel kontrast artırma
            Mat claheSonuc = new Mat();
            var clahe = Cv2.CreateCLAHE(3.0, new OpenCvSharp.Size(8, 8));
            clahe.Apply(gri, claheSonuc);

            // 2. Hafif gürültü azaltma
            Mat temiz = new Mat();
            Cv2.BilateralFilter(claheSonuc, temiz, 5, 75, 75);

            // 3. Yazıları keskinleştirme
            Mat blur = new Mat();
            Cv2.GaussianBlur(temiz, blur, new OpenCvSharp.Size(0, 0), 1.0);

            Mat keskin = new Mat();
            Cv2.AddWeighted(temiz, 1.5, blur, -0.5, 0, keskin);

            return keskin;
        }
        public Mat PrepareForOCR(Mat kaynak)
        {
            Mat gri = ToGray(kaynak);

            // Arka planı yumuşatarak tahmin et
            Mat background = new Mat();
            Cv2.GaussianBlur(gri, background, new OpenCvSharp.Size(51, 51), 0);

            // Gölge / arka plan etkisini azalt
            Mat normalized = new Mat();
            Cv2.Divide(gri, background, normalized, 255);

            // Kontrastı artır
            Mat claheSonuc = new Mat();
            var clahe = Cv2.CreateCLAHE(3.0, new OpenCvSharp.Size(8, 8));
            clahe.Apply(normalized, claheSonuc);

            // Hafif keskinleştir
            Mat blur = new Mat();
            Cv2.GaussianBlur(claheSonuc, blur, new OpenCvSharp.Size(0, 0), 1.0);

            Mat keskin = new Mat();
            Cv2.AddWeighted(claheSonuc, 1.6, blur, -0.6, 0, keskin);

            // OCR için siyah-beyaz hale getir
            Mat sonuc = new Mat();
            Cv2.AdaptiveThreshold(
                keskin,
                sonuc,
                255,
                AdaptiveThresholdTypes.GaussianC,
                ThresholdTypes.Binary,
                41,
                7);

            return sonuc;
        }
    }
}