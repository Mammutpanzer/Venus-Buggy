using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace VenusBuggy
{
    class Panel
    {
        
        public Bitmap bmp;
        public byte alpha;
        public Panel()
        {
            bmp = null;
            alpha = 0;
        }
    }
}
