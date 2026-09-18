using BelgeArsivlemeSistemi.Models;
using OpenCvSharp;

namespace BelgeArsivlemeSistemi.ImageProcessing
{
    public class DocumentAnalyzer
    {
        public DocumentAnalysisResult Analyze(Mat kaynak)
        {
            Mat gri = new Mat();

            if (kaynak.Channels() == 3)
                Cv2.CvtColor(kaynak, gri, ColorConversionCodes.BGR2GRAY);
            else
                gri = kaynak.Clone();

            Scalar mean = Cv2.Mean(gri);
            double brightness = mean.Val0;

            Scalar meanValue, stdValue;

            // Kontrast
            Cv2.MeanStdDev(gri, out meanValue, out stdValue);
            double contrast = stdValue.Val0;

            // Bulanıklık
            Mat laplacian = new Mat();
            Cv2.Laplacian(gri, laplacian, MatType.CV_64F);

            Scalar lapMean, lapStd;
            Cv2.MeanStdDev(laplacian, out lapMean, out lapStd);

            double blur = lapStd.Val0 * lapStd.Val0;

            string quality;

            if (brightness < 80)
                quality = "Karanlık";
            else if (contrast < 35)
                quality = "Düşük Kontrast";
            else if (blur < 100)
                quality = "Bulanık";
            else
                quality = "İyi";

            return new DocumentAnalysisResult
            {
                Brightness = brightness,
                Contrast = contrast,
                Blur = blur,

                BrightnessStatus = brightness < 80 ? "Düşük" :
                        brightness > 180 ? "Yüksek" : "Normal",

                ContrastStatus = contrast < 25 ? "Düşük" :
                      contrast < 50 ? "Normal" : "İyi",

                BlurStatus = blur < 50 ? "Düşük" :
                  blur < 150 ? "Normal" : "İyi",

                Quality = quality
            };
        }
    }
}