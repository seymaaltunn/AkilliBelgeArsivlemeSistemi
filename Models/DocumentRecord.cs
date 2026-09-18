using System;

namespace BelgeArsivlemeSistemi.Models
{
    public class DocumentRecord
    {
        public int Id { get; set; }

        public string BelgeAdi { get; set; } = "";
        public string BelgeTuru { get; set; } = "";
        public string KisiKurum { get; set; } = "";
        public string KimlikNo { get; set; } = "";
        public string Kurum { get; set; } = "";
        public DateTime? BelgeTarihi { get; set; }
        public string EvrakNo { get; set; } = "";
        public string Aciklama { get; set; } = "";
        public string DosyaYolu { get; set; } = "";
        public string OcrMetni { get; set; } = "";
        public DateTime KayitTarihi { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
    }
}
