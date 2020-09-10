using System;
using System.Drawing;

namespace Common
{
    [Serializable]
    public class ImageBit
    {
        public Bitmap Imagebit { get; set; }
        public ImageBit(Bitmap bitmap)
        {
            Imagebit = bitmap;
        }
    }
}
