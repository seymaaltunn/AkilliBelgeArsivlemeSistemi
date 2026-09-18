using OpenCvSharp;
using System.Linq;



namespace BelgeArsivlemeSistemi.AI
{ 
    public class DocumentVisibilityService
    {
        public bool TamGorunuyorMu(
    Rect rect,
    Mat kirpilmisBelge,
    string ocrMetni,
    int resimGenisligi,
    int resimYuksekligi)
        {
            if (rect.Width <= 0 ||
                rect.Height <= 0 ||
                resimGenisligi <= 0 ||
                resimYuksekligi <= 0)
            {
                return false;
            }

            const int kenarPayi = 5;

            
            int minimumGenislik = Math.Max(40, resimGenisligi / 20);
            int minimumYukseklik = Math.Max(40, resimYuksekligi / 20);

            if (rect.Width < minimumGenislik ||
                rect.Height < minimumYukseklik)
            {
                return false;
            }

            bool kenaraDegiyor =
                rect.X <= kenarPayi ||
                rect.Y <= kenarPayi ||
                rect.Right >= resimGenisligi - kenarPayi ||
                rect.Bottom >= resimYuksekligi - kenarPayi;

            double alanOrani =
                (double)rect.Width * rect.Height /
                ((double)resimGenisligi * resimYuksekligi);

            bool cokKucuk = alanOrani < 0.02;

            double enBoyOrani =
                (double)rect.Width / rect.Height;

            bool asiriDar =
                enBoyOrani < 0.28 ||
                enBoyOrani > 3.8;

            int karakterSayisi = string.IsNullOrWhiteSpace(ocrMetni)
                ? 0
                : ocrMetni.Count(char.IsLetterOrDigit);

            bool metinCokAz = karakterSayisi < 15;

            
            if (cokKucuk && asiriDar)
                return false;

            
            if (kenaraDegiyor && asiriDar)
                return false;

           
            if (asiriDar && metinCokAz)
                return false;

            int sorunPuani = 0;

            if (kenaraDegiyor)
                sorunPuani++;

            if (cokKucuk)
                sorunPuani++;

            if (asiriDar)
                sorunPuani++;

            if (metinCokAz)
                sorunPuani++;

            
            return sorunPuani < 2;
        }
    }
    }