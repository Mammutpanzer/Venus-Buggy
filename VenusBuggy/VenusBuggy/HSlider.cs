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
    class HSlider
    {
        public int length;
        public int margin;
        public int Height;

        public Point pos;

        public int endWidth;
        public int endHeight;

        public Bitmap bmp_end;
        public Bitmap bmp_bar;
        public Bitmap bmp_off;
        public Bitmap bmp_over;
        public Bitmap bmp_click;

        public int tex_end;
        public int tex_bar;
        public int tex_off;
        public int tex_over;
        public int tex_click;

        public int tex_active;

        public int result;

        public HSlider(int x, int y, int length, int margin, int Height, int end_w, int end_h, int bar_h, int slider_w, int slider_h, int result, string filename_end, string filename_bar, string filename_off, string filename_over, string filename_click)
        {
            bmp_end = new Bitmap(filename_end);
            bmp_bar = new Bitmap(filename_bar);
            bmp_off = new Bitmap(filename_off);
            bmp_over = new Bitmap(filename_over);
            bmp_click = new Bitmap(filename_click);

            bitmapOverdriveTransparency();

            tex_end = loadTexture(bmp_end);
            tex_bar = loadTexture(bmp_bar);
            tex_off = loadTexture(bmp_off);
            tex_over = loadTexture(bmp_over);
            tex_click = loadTexture(bmp_click);

            tex_active = tex_off;

            pos.X = x;
            pos.Y = y;
            this.length = length;
            this.margin = margin;
            this.Height = Height;

            endWidth = end_w;
            endHeight = end_h;

            this.result = result;
        }

        public void bitmapOverdriveTransparency()       //Schalte Transparenz für Magenta-des-Todes ein
        {
            bmp_end.MakeTransparent(Color.FromArgb(0, 255, 0, 255));
            bmp_bar.MakeTransparent(Color.FromArgb(0, 255, 0, 255));
            bmp_off.MakeTransparent(Color.FromArgb(0, 255, 0, 255));
            bmp_over.MakeTransparent(Color.FromArgb(0, 255, 0, 255));
            bmp_click.MakeTransparent(Color.FromArgb(0, 255, 0, 255));
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

        public void draw() //############################## Hier Target oder so hinzufügen (Slider nimmt erstmal die Position des jeweiligen Werts an.
        {
            GL.BindTexture(TextureTarget.Texture2D, tex_bar);
            GL.Begin(PrimitiveType.Quads);                          //Muss erst getestet werden
            GL.TexCoord2(0, 1);     
            GL.Vertex2(             pos.X - margin,               pos.Y + (int)(endHeight/2) - (int)(Height/2));
            GL.TexCoord2(1, 1);
            GL.Vertex2(             pos.X + margin + length,      pos.Y + (int)(endHeight/2) - (int)(Height/2));
            GL.TexCoord2(1, 0);
            GL.Vertex2(             pos.X + margin + length,      pos.Y + (int)(endHeight/2) + (int)(Height/2));
            GL.TexCoord2(0, 0);
            GL.Vertex2(             pos.X - margin,               pos.Y + (int)(endHeight/2) + (int)(Height/2));
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, tex_end);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1);
            GL.Vertex2(             pos.X - endWidth + margin,      pos.Y);
            GL.TexCoord2(1, 1);
            GL.Vertex2(             pos.X + margin,                 pos.Y);
            GL.TexCoord2(1, 0);
            GL.Vertex2(             pos.X + margin,                 pos.Y + endHeight);
            GL.TexCoord2(0, 0);
            GL.Vertex2(             pos.X - endWidth + margin,      pos.Y + endHeight);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, tex_end);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(1, 0);
            GL.Vertex2(             pos.X - margin + length,                            pos.Y);
            GL.TexCoord2(0, 0);
            GL.Vertex2(             pos.X + endWidth - margin + length,                 pos.Y);
            GL.TexCoord2(0, 1);
            GL.Vertex2(             pos.X + endWidth - margin + length,                 pos.Y + endHeight);
            GL.TexCoord2(1, 1);
            GL.Vertex2(             pos.X - margin + length,                            pos.Y + endHeight);
            GL.End();

            //GL.BindTexture(TextureTarget.Texture2D, tex_active);
            //GL.Begin(PrimitiveType.Quads);
            //GL.TexCoord2(0, 1);
            //GL.Vertex2(pos.X, pos.Y);
            //GL.TexCoord2(1, 1);
            //GL.Vertex2(pos.X + Width, pos.Y);
            //GL.TexCoord2(1, 0);
            //GL.Vertex2(pos.X + Width, pos.Y + Height);
            //GL.TexCoord2(0, 0);
            //GL.Vertex2(pos.X, pos.Y + Height);
            //GL.End();
        }
    }
}
