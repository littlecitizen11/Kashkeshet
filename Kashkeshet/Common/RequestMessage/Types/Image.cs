using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

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
