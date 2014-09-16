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
            short Width = 1366; //Fenstergröße
            short Height = 768;
            var Border = WindowBorder.Hidden;   //Rahmen
            var Fullscreen = WindowState.Fullscreen; //Vollbildmodus

            int tex_test = 0;
            

            var BGColor = new Color(); //Hintergrundfarbe       BGColor = Color.FromArgb(0, 128, 128, 128);


            using (var app = new GameWindow())
            {
                app.Load += (sender, e) =>
                {

                    app.Width = Width;   
                    app.Height = Height;
                    app.WindowBorder = Border;     
                    app.WindowState = Fullscreen;

                    tex_test = game.loadTexture("texturen/hello2.jpg");


                    GL.Enable(EnableCap.Texture2D); //Erlaube 2D-Teturierung

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



                    GL.Begin(PrimitiveType.Quads);
                    GL.BindTexture(TextureTarget.Texture2D, tex_test);
                    GL.TexCoord2(0, 0);
                    GL.Vertex2(000.0f, 1080.0f);
                    GL.TexCoord2(1, 0);
                    GL.Vertex2(1920.0f, 1080.0f);
                    GL.TexCoord2(1, 1);
                    GL.Vertex2(1920.0f, 00.0f);
                    GL.TexCoord2(0, 1);
                    GL.Vertex2(00.0f, 00.0f);



                    GL.End();

                    app.SwapBuffers();
                };

                app.Run(60.0);  //Updatefrequenz - Drawing wird notfalls übersprungen
            }
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
    }
}
