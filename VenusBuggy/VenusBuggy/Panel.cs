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
        public int Width;
        public int Height;

        public Point pos;

        public Bitmap bmp_off;
        public Bitmap bmp_over;
        public Bitmap bmp_click;

        public int tex_off;
        public int tex_over;
        public int tex_click;

        public int tex_active;

        public int result;

        public Panel(int x, int y, int w, int h, int r, string filename_off, string filename_over, string filename_click)
        {
            bmp_off = new Bitmap(filename_off);
            bmp_over = new Bitmap(filename_over);
            bmp_click = new Bitmap(filename_click);

            bitmapOverdriveTransparency();

            tex_off = loadTexture(bmp_off);
            tex_over = loadTexture(bmp_over);
            tex_click = loadTexture(bmp_click);

            tex_active = tex_off;

            pos.X = x;
            pos.Y = y;
            Width = w;
            Height = h;

            result = r;
        }
        public Panel(int x, int y, int w, int h, int r, string filename_off, string filename_over)
        {
            bmp_off = new Bitmap(filename_off);
            bmp_over = new Bitmap(filename_over);
            bmp_click = null;

            bitmapOverdriveTransparency();

            tex_off = loadTexture(bmp_off);
            tex_over = loadTexture(bmp_over);
            tex_click = 0;

            tex_active = tex_off;

            pos.X = x;
            pos.Y = y;
            Width = w;
            Height = h;

            result = r;
        }
        public Panel(int x, int y, int w, int h, int r, string filename_off)
        {
            bmp_off = new Bitmap(filename_off);
            bmp_over = null;
            bmp_click = null;

            bitmapOverdriveTransparency();

            tex_off = loadTexture(bmp_off);
            tex_over = 0;
            tex_click = 0;

            tex_active = tex_off;

            pos.X = x;
            pos.Y = y;
            Width = w;
            Height = h;

            result = r;
        }

        public void bitmapOverdriveTransparency()
        {
            bmp_off.MakeTransparent(Color.FromArgb(0, 255, 0, 255));
            if (bmp_over != null)
            {
                bmp_over.MakeTransparent(Color.FromArgb(0, 255, 0, 255));
            }
            if (bmp_click != null)
            {
                bmp_click.MakeTransparent(Color.FromArgb(0, 255, 0, 255));
            }
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

        public void setFull(int x, int y, int w, int h, int r)
        {
            this.Width = w;
            this.Height = h;
            this.pos.X = x;
            this.pos.Y = y;

            this.result = r;
        }

        //public void setBitmapOverdrive(string filename)
        //{
        //    bmp = new Bitmap(filename);
        //    tex = loadTexture(bmp);
        //}

        //public void setFullBitmapOverdrive(int x, int y, int w, int h, byte a, string filename)
        //{
        //    this.Width = w;
        //    this.Height = h;
        //    this.pos.X = x;
        //    this.pos.Y = y;
        //    bmp = new Bitmap(filename);
        //    tex = loadTexture(bmp);
        //}



        public void draw()
        {
            GL.BindTexture(TextureTarget.Texture2D, tex_active);
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

        //############################################################# Check-Event einfügen und Konstruktor um Rückgabewert 'int result' erweitern
        public int clickCheck(int x, int y, MouseState state, int cron)
        {


            if (((x >= pos.X) && (x <= (pos.X + Width))) && ((y >= pos.Y) && (y <= (pos.Y + Height))))  //Ist der Cursor im Panel-Feld
            {
                switch (state[MouseButton.Left])    //wurde das Panel auch angeklickt
                {
                    case true:
                        if (tex_click != 0)
                        {
                            tex_active = tex_click;
                        }
                        return result;

                    case false:
                        if (tex_over != 0)
                        {
                            tex_active = tex_over;
                        }
                        break;
                }

            }
            else
            {
                tex_active = tex_off;
            }
            return cron;        //sonst gebe den bereits vorhandenen Cron wieder zurück
        }
    }
}
