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
        public Bitmap bmp_end;
        public Bitmap bmp_bar;
        public Bitmap bmp_off;
        public Bitmap bmp_over;
        public Bitmap bmp_click;

        public Point pos;

        public int barWidth;
        public int barHeight;

        public int endWidth;
        public int endHeight;
        public int endMargin;

        public int sliderWidth;
        public int sliderHeight;
        private Point sliderPos;

        public int value;

        public bool clickLock;

        public int tex_end;
        public int tex_bar;
        public int tex_off;
        public int tex_over;
        public int tex_click;
        public int tex_active;

        /// <summary>
        /// Erstellt einen horizontalen Slider
        /// </summary>
        /// <param name="value">Startvalue, an den der Slider prozentual gesetzt wird. Wertebereich von 0 bis 100. (Anzusehen in Prozent)</param>
        /// <param name="posX">Linke Ecke der Bar.</param>
        /// <param name="posY">Untere Ecke der Bar.</param>
        /// <param name="barWidth">Länge der Bar.</param>
        /// <param name="endMargin">Einzug der Endstücke nach innen.</param>
        /// <param name="barHeight">Höhe der Bar.</param>
        /// <param name="endWidth">Breite des Endstücks</param>
        /// <param name="endHeight">Höhe des Endstücks</param>
        /// <param name="sliderWidth">Breite des Sliders</param>
        /// <param name="sliderHeight">Höhe des Sliders</param>
        /// <param name="filename_end">Dateipfad</param>
        /// <param name="filename_bar">Dateipfad</param>
        /// <param name="filename_off">Dateipfad</param>
        /// <param name="filename_over">Dateipfad</param>
        /// <param name="filename_click">Dateipfad</param>
        public HSlider(int value, int posX, int posY, int barWidth, int endMargin, int barHeight, int endWidth, int endHeight, int sliderWidth, int sliderHeight, string filename_end, string filename_bar, string filename_off, string filename_over, string filename_click)
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

            this.pos.X = posX;
            this.pos.Y = posY;

            this.barWidth = barWidth;
            this.barHeight = barHeight;

            this.endWidth = endWidth;
            this.endHeight = endHeight;
            this.endMargin = endMargin;

            this.sliderWidth = sliderWidth;
            this.sliderHeight = sliderHeight;

            this.value = value;

            this.clickLock = false;
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

        public void draw()
        {
            GL.BindTexture(TextureTarget.Texture2D, tex_bar);
            GL.Begin(PrimitiveType.Quads);                          
            GL.TexCoord2(0, 1);     
            GL.Vertex2(             pos.X - endMargin,                  pos.Y + (int)(endHeight/2) - (int)(barHeight/2));
            GL.TexCoord2(1, 1);
            GL.Vertex2(             pos.X + endMargin + barWidth,       pos.Y + (int)(endHeight / 2) - (int)(barHeight / 2));
            GL.TexCoord2(1, 0);
            GL.Vertex2(             pos.X + endMargin + barWidth,       pos.Y + (int)(endHeight / 2) + (int)(barHeight / 2));
            GL.TexCoord2(0, 0);
            GL.Vertex2(             pos.X - endMargin,                  pos.Y + (int)(endHeight / 2) + (int)(barHeight / 2));
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, tex_end);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1);
            GL.Vertex2(             pos.X - endWidth + endMargin,       pos.Y);
            GL.TexCoord2(1, 1);
            GL.Vertex2(             pos.X + endMargin,                  pos.Y);
            GL.TexCoord2(1, 0);
            GL.Vertex2(             pos.X + endMargin,                  pos.Y + endHeight);
            GL.TexCoord2(0, 0);
            GL.Vertex2(             pos.X - endWidth + endMargin,       pos.Y + endHeight);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, tex_end);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(1, 0);
            GL.Vertex2(             pos.X - endMargin + barWidth,               pos.Y);
            GL.TexCoord2(0, 0);
            GL.Vertex2(             pos.X + endWidth - endMargin + barWidth,    pos.Y);
            GL.TexCoord2(0, 1);
            GL.Vertex2(             pos.X + endWidth - endMargin + barWidth,    pos.Y + endHeight);
            GL.TexCoord2(1, 1);
            GL.Vertex2(             pos.X - endMargin + barWidth,               pos.Y + endHeight);
            GL.End();



            GL.BindTexture(TextureTarget.Texture2D, tex_active);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(1, 0);
            GL.Vertex2(             (int)(pos.X + (barWidth / 100 * value) - (int)(sliderWidth / 2)),       pos.Y + (int)(barHeight / 2) - (int)(sliderHeight / 2));
            GL.TexCoord2(0, 0);
            GL.Vertex2(             (int)(pos.X + (barWidth / 100 * value) + (int)(sliderWidth / 2)),       pos.Y + (int)(barHeight / 2) - (int)(sliderHeight / 2));
            GL.TexCoord2(0, 1);
            GL.Vertex2(             (int)(pos.X + (barWidth / 100 * value) + (int)(sliderWidth / 2)),       pos.Y + (int)(barHeight / 2) + (int)(sliderHeight / 2));
            GL.TexCoord2(1, 1);
            GL.Vertex2(             (int)(pos.X + (barWidth / 100 * value) - (int)(sliderWidth / 2)),       pos.Y + (int)(barHeight / 2) + (int)(sliderHeight / 2));
            GL.End();
        }


        public int clickCheck(int x, int y, MouseState state, int value)
        {
            sliderPos.X = (int)(pos.X + (barWidth / 100 * value) - (int)(sliderWidth / 2));
            sliderPos.Y = pos.Y + (int)(barHeight / 2) - (int)(sliderHeight / 2);

            // wenn die übergebenen Koordinaten im Feld des aktiven Images liegen
            if ((((x >= pos.X) && (x <= (pos.X + barWidth))) && ((y >= pos.Y) && (y <= (pos.Y + barHeight))))          || (clickLock)) //Ist der Cursor in der Bar
            //if (((x >= sliderPos.X) && (x <= (sliderPos.X + sliderWidth))) && ((y >= sliderPos.Y) && (y <= (sliderPos.Y + sliderHeight))))
            {
                if (state[MouseButton.Left])    //wurde das Panel auch angeklickt
                {
                    if (tex_click != 0)
                    {
                        tex_active = tex_click;
                    }
                    clickLock = true;
                }
                else
                {
                    if (tex_over != 0)
                    {
                        tex_active = tex_over;
                    }
                }
            }
            else
            {
                tex_active = tex_off;
            }
            if (clickLock)
            {

                value = (int)((x - pos.X) * 100 / barWidth);

                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                this.value = value;
            }
            if ((!state[MouseButton.Left]) && (clickLock))
            {
                this.value = value;
                clickLock = false;
            }
            return value;
        }
    }
}
