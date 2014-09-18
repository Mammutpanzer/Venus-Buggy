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
        public int tex;
        public Bitmap bmp;

        public byte alpha;

        public Point pos;
        public int Width;
        public int Height;

        public Panel(int x, int y, int w, int h, byte a, string filename)
        {
            bmp = new Bitmap(filename);
            tex = loadTexture(bmp);
            alpha = a;
            pos.X = x;
            pos.Y = y;
            Width = w;
            Height = h;
        }

        public int loadTexture(Bitmap bmp)    //Fertiger Texturenlader - Gibt die Speicher-ID zurück
        {
            int id = GL.GenTexture();       //Nimm die Textur und gib ihr eine ID
            GL.BindTexture(TextureTarget.Texture2D, id);

            //Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                bmp_data.Width, bmp_data.Height,
                0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                bmp_data.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            bmp.UnlockBits(bmp_data);

            return id;
        }

        public void setPos(int x, int y)
        {
            this.Width = x;
            this.Height = y;
        }

        public void setWidth(int w)
        {
            this.Width = w;
        }

        public void setHeight(int h)
        {
            this.Height = h;
        }

        public void setAlpha(byte a)
        {
            this.alpha = a;
        }

        public void setFull(int x, int y, int w, int h, byte a)
        {
            this.alpha = a;
            this.Width = w;
            this.Height = h;
            this.pos.X = x;
            this.pos.Y = y;
        }

        public void setBitmapOverdrive(string filename)
        {
            bmp = new Bitmap(filename);
            tex = loadTexture(bmp);
        }

        public void setFullBitmapOverdrive(int x, int y, int w, int h, byte a, string filename)
        {
            this.alpha = a;
            this.Width = w;
            this.Height = h;
            this.pos.X = x;
            this.pos.Y = y;
            bmp = new Bitmap(filename);
            tex = loadTexture(bmp);
        }

        public void draw()
        {
            GL.BindTexture(TextureTarget.Texture2D, tex);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1);
            GL.Vertex2(pos.X, pos.Y);
            GL.TexCoord2(1, 1);
            GL.Vertex2(pos.X + Width, pos.Y);
            GL.TexCoord2(1, 0);
            GL.Vertex2(pos.X + Width, pos.Y + Height);
            GL.TexCoord2(0, 0);
            GL.Vertex2(pos.X, pos.Y + Height);
            GL.End();
        }
    }
}
