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
    class Application
    {

        //public int loadTexture(string filename);


        [STAThread] //Single Threaded Appartment
        public static void Main()
        {
            Application game = new Application();   //Genereller Objektverweis. Hiermit kann auf nicht statische Objekte zugegriffen werden via game.example(x, y)


            int cron = 0;   //Globale Steuervariable

            short Width = 1366; //Fenstergröße
            short Height = 768;

            var Border = WindowBorder.Hidden;   //Rahmen
            var Fullscreen = WindowState.Fullscreen; //Vollbildmodus


            int tex_bg = 0;
            int tex_end = 0;

            var BGColor = new Color(); //Hintergrundfarbe       BGColor = Color.FromArgb(0, 128, 128, 128);


            using (var app = new GameWindow())
            {
                app.Load += (sender, e) =>
                {

                    app.Width = Width;   
                    app.Height = Height;
                    app.WindowBorder = Border;     
                    app.WindowState = Fullscreen;

                    tex_bg = game.getMenuBG(Width);
                    tex_end = game.loadTexture2("texturen/MenuEnd.png");

                    Console.WriteLine(tex_bg);
                    Console.WriteLine(tex_end);

                    GL.Enable(EnableCap.Texture2D); //Erlaube 2D-Teturierung

                    GL.Enable(EnableCap.Blend); //Alpha-Kanäle aktivieren
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                    BGColor = Color.FromArgb(0, 128, 128, 128);

                    app.VSync = VSyncMode.On;
                };

                app.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, app.Width, app.Height);
                };

                app.UpdateFrame += (sender, e) =>
                {
                    if (app.Keyboard[Key.Escape])
                    {
                        app.Exit();
                    }
                };

                app.RenderFrame += (sender, e) =>
                {
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();
                    GL.Ortho(0, app.Width, 0, app.Height, 0.0, 4.0);  //Nullpunkt ist unten links!

                    GL.ClearColor(BGColor);

                    switch (cron)
                    {
                        case 0:

                            game.drawMenu(tex_bg, tex_end);
                            break;
                    }





                    app.SwapBuffers();
                };

                app.Run(60.0);  //Updatefrequenz - Drawing wird notfalls übersprungen
            }
        }

        private void drawMenu(int bg, int end)
        {
            GL.Begin(PrimitiveType.Quads);
            GL.BindTexture(TextureTarget.Texture2D, bg);
                GL.TexCoord2(0, 0);
                GL.Vertex2(000.0f, 1080.0f);
                GL.TexCoord2(1, 0);
                GL.Vertex2(1920.0f, 1080.0f);
                GL.TexCoord2(1, 1);
                GL.Vertex2(1920.0f, 00.0f);
                GL.TexCoord2(0, 1);
                GL.Vertex2(00.0f, 00.0f);
            GL.End();


            GL.Begin(PrimitiveType.Quads);
            GL.BindTexture(TextureTarget.Texture2D, end);
                GL.TexCoord2(0, 0);
                GL.Vertex2(100.0f, 144.0f);
                GL.TexCoord2(1, 0);
                GL.Vertex2(325.0f, 144.0f);
                GL.TexCoord2(1, 1);
                GL.Vertex2(325.0f, 100.0f);
                GL.TexCoord2(0, 1);
                GL.Vertex2(100.0f, 100.0f);
            GL.End();
        }

        private int getMenuBG(int Width) // Holt sich je nach Startauflösung die korrekte Hintergrundgröße
        {
            int tex;
            if (Width < 1280)
                tex = loadTexture("texturen/MenuBG_1024_768.jpg");
            else if (Width < 1366)
                tex = loadTexture("texturen/MenuBG_1280_1024.jpg");
            else if (Width < 1440)
                tex = loadTexture("texturen/MenuBG_1366_768.jpg");
            else if (Width < 1920)
                tex = loadTexture("texturen/MenuBG_1440_900.jpg");
            else
                tex = loadTexture("texturen/MenuBG_1920_1080.jpg");

            return tex;
        }

        private int loadTexture(string filename)    //Fertiger Texturenlader
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            if (!File.Exists(filename))
            {
                Console.WriteLine("FEHLER: Die Datei '" + filename + "' existiert nicht im Verzeichnis!");
                Console.WriteLine("Prüfen Sie das Vorhandensein dieser Datei bzw. die Richtigkeit des Dateinamens oder der Dateiendung.");
                return 0;
            }

            int id = GL.GenTexture();       //Nimm die Textur und gib ihr eine ID
            //GL.BindTexture(TextureTarget.Texture2D, id);

            // Und den Rest hiervon verstehe ich nicht
            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return id;
        }
        private int loadTexture2(string filename)    //Fertiger Texturenlader
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            if (!File.Exists(filename))
            {
                Console.WriteLine("FEHLER: Die Datei '" + filename + "' existiert nicht im Verzeichnis!");
                Console.WriteLine("Prüfen Sie das Vorhandensein dieser Datei bzw. die Richtigkeit des Dateinamens oder der Dateiendung.");
                return 0;
            }

            int id = GL.GenTexture();       //Nimm die Textur und gib ihr eine ID
            //GL.BindTexture(TextureTarget.Texture2D, id);

            // Und den Rest hiervon verstehe ich nicht
            Bitmap bmp2 = new Bitmap(filename);
            BitmapData bmp_data = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp2.UnlockBits(bmp_data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return id;
        }
    }
}
