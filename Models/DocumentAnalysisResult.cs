namespace BelgeArsivlemeSistemi.Models
{
    public class DocumentAnalysisResult
    {
        public double Brightness { get; set; }

        public double Contrast { get; set; }

        public double Blur { get; set; }

        public string Quality { get; set; } = "";

        public string BrightnessStatus { get; set; } = "";

        public string ContrastStatus { get; set; } = "";

        public string BlurStatus { get; set; } = "";
    }
}