using OpenCvSharp;
using System;
using System.Linq;

namespace BelgeArsivlemeSistemi.ImageProcessing
{
    public class PerspectiveCorrection
    {
        /// <summary>
        /// Belgeyi perspektif açısından düzeltmeye çalışır.
        /// Başarılı olursa true döner ve sonuc düzeltilmiş görüntüyü içerir.
        /// Başarısız olursa false döner ve sonuc orijinal görüntünün klonu olur
        /// (çağıran taraf başarısızlığı fark edip buna göre davranabilir).
        /// </summary>
        public bool Duzelt(Mat belge, out Mat sonuc)
        {
            Mat kaynak = belge.Clone();

            using Mat gri = new Mat();

            if (kaynak.Channels() == 3)
            {
                Cv2.CvtColor(kaynak, gri, ColorConversionCodes.BGR2GRAY);
            }
            else
            {
                kaynak.CopyTo(gri);
            }

            using Mat blur = new Mat();
            Cv2.GaussianBlur(gri, blur, new OpenCvSharp.Size(5, 5), 0);

            using Mat kenarlar = new Mat();
            Cv2.Canny(blur, kenarlar, 40, 140);

            using Mat kernel = Cv2.GetStructuringElement(
                MorphShapes.Rect,
                new OpenCvSharp.Size(5, 5));

            Cv2.Dilate(kenarlar, kenarlar, kernel, iterations: 1);

            Cv2.FindContours(
                kenarlar,
                out OpenCvSharp.Point[][] contours,
                out _,
                RetrievalModes.External,
                ContourApproximationModes.ApproxSimple);

            if (contours.Length == 0)
            {
                sonuc = kaynak;
                return false;
            }

            
            double minAlan = kaynak.Width * kaynak.Height * 0.10;

            OpenCvSharp.Point[]? enIyiContour = null;
            double enBuyukAlan = 0;

            foreach (var contour in contours)
            {
                double alan = Cv2.ContourArea(contour);

                if (alan < minAlan)
                    continue;

                double cevre = Cv2.ArcLength(contour, true);

                for (double oran = 0.01; oran <= 0.08; oran += 0.01)
                {
                    OpenCvSharp.Point[] approx =
                        Cv2.ApproxPolyDP(contour, oran * cevre, true);

                    if (approx.Length == 4 && alan > enBuyukAlan)
                    {
                        enBuyukAlan = alan;
                        enIyiContour = approx;
                        break;
                    }
                }
            }

            if (enIyiContour == null)
            {
                sonuc = kaynak;
                return false;
            }

            Point2f[] sirali = KoseleriSirala(enIyiContour);

            if (!GecerliDortgenMi(sirali))
            {
                sonuc = kaynak;
                return false;
            }

            Mat warpSonucu = PerspektifUygula(kaynak, sirali, out bool basarili);

            if (!basarili)
            {
                warpSonucu.Dispose();
                sonuc = kaynak;
                return false;
            }

            kaynak.Dispose();
            sonuc = warpSonucu;
            return true;
        }

        private Mat PerspektifUygula(Mat kaynak, Point2f[] sirali, out bool basarili)
        {
            float genislikUst = Mesafe(sirali[0], sirali[1]);
            float genislikAlt = Mesafe(sirali[3], sirali[2]);
            float genislik = Math.Max(genislikUst, genislikAlt);

            float yukseklikSol = Mesafe(sirali[0], sirali[3]);
            float yukseklikSag = Mesafe(sirali[1], sirali[2]);
            float yukseklik = Math.Max(yukseklikSol, yukseklikSag);

            if (genislik < 80 || yukseklik < 80)
            {
                basarili = false;
                return kaynak.Clone();
            }

            Point2f[] hedef =
            {
                new Point2f(0, 0),
                new Point2f(genislik - 1, 0),
                new Point2f(genislik - 1, yukseklik - 1),
                new Point2f(0, yukseklik - 1)
            };

            using Mat matrix = Cv2.GetPerspectiveTransform(sirali, hedef);

            Mat sonuc = new Mat();

            Cv2.WarpPerspective(
                kaynak,
                sonuc,
                matrix,
                new OpenCvSharp.Size((int)genislik, (int)yukseklik));

            basarili = true;
            return sonuc;
        }

        /// <summary>
        /// Merkez noktaya göre açı hesaplayarak köşeleri saat yönünde sıralar
        /// (sol üst, sağ üst, sağ alt, sol alt). x+y / y-x yaklaşımı 45 dereceye
        /// yakın döndürülmüş belgelerde yanlış eşleşme yapabildiği için
        /// açı bazlı sıralamaya geçildi.
        /// </summary>
        private Point2f[] KoseleriSirala(OpenCvSharp.Point[] pts)
        {
            var noktalar = pts.Select(p => new Point2f(p.X, p.Y)).ToArray();

            float merkezX = noktalar.Average(p => p.X);
            float merkezY = noktalar.Average(p => p.Y);

            var aciyaGoreSirali = noktalar
                .OrderBy(p => Math.Atan2(p.Y - merkezY, p.X - merkezX))
                .ToArray();

            // Açıya göre sıralama saat yönünün tersine (matematiksel) başlar;
            // en küçük x+y değerine sahip noktayı "sol üst" kabul edip
            // listeyi ona göre kaydırıyoruz.
            int solUstIndex = 0;
            float enKucukToplam = float.MaxValue;

            for (int i = 0; i < aciyaGoreSirali.Length; i++)
            {
                float toplam = aciyaGoreSirali[i].X + aciyaGoreSirali[i].Y;

                if (toplam < enKucukToplam)
                {
                    enKucukToplam = toplam;
                    solUstIndex = i;
                }
            }

            Point2f[] sonuc = new Point2f[4];

            for (int i = 0; i < 4; i++)
            {
                sonuc[i] = aciyaGoreSirali[(solUstIndex + i) % 4];
            }

            return sonuc;
        }

        private bool GecerliDortgenMi(Point2f[] sirali)
        {
            if (sirali.Length != 4)
                return false;

            // Kenar uzunlukları çok küçükse ya da iki köşe üst üste
            // biniyorsa (dejenere dörtgen) geçersiz say.
            for (int i = 0; i < 4; i++)
            {
                float uzunluk = Mesafe(sirali[i], sirali[(i + 1) % 4]);

                if (uzunluk < 10)
                    return false;
            }

            return true;
        }

        private float Mesafe(Point2f p1, Point2f p2)
        {
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;

            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}