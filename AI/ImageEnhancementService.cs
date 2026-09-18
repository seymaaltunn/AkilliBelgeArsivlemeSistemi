using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BelgeArsivlemeSistemi.AI
{
    public class ImageEnhancementService : IDisposable
    {
        private readonly InferenceSession? session;

        public ImageEnhancementService()
        {
            string modelPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "AI",
                "Enhancement",
                "enhancer.onnx");

            if (File.Exists(modelPath))
            {
                session = new InferenceSession(modelPath);
            }
        }

        public Mat Enhance(Mat kaynak)
        {
           
            if (session == null)
            {
                Mat sonuc = new Mat();

                Cv2.DetailEnhance(kaynak, sonuc, 10f, 0.15f);

                return sonuc;
            }

            
            return kaynak.Clone();
        }

        public void Dispose()
        {
            session?.Dispose();
        }
    }
}