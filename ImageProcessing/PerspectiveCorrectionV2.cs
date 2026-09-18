using OpenCvSharp;
using BelgeArsivlemeSistemi.ImageProcessing;


namespace BelgeArsivlemeSistemi.ImageProcessing
{
    public class PerspectiveCorrectionV2
    {
        public Mat Duzelt(Mat belge)
        {
            Mat kaynak = belge.Clone();

            Mat gri = new Mat();

            if (kaynak.Channels() == 3)
                Cv2.CvtColor(kaynak, gri, ColorConversionCodes.BGR2GRAY);
            else
                gri = kaynak.Clone();

            Mat claheSonuc = new Mat();
            var clahe = Cv2.CreateCLAHE(2.0, new OpenCvSharp.Size(8, 8));
            clahe.Apply(gri, claheSonuc);

            Point2f[] corners = Cv2.GoodFeaturesToTrack(
     claheSonuc,
     40,
     0.01,
     20,
     null,
     3,
     false,
     0.04);

            Mat sonuc = kaynak.Clone();

            if (corners != null)
            {
                foreach (var corner in corners)
                {
                    Cv2.Circle(
                        sonuc,
                        new OpenCvSharp.Point((int)corner.X, (int)corner.Y),
                        6,
                        Scalar.Red,
                        -1);
                }
            }

            return sonuc;
        }
    }
}