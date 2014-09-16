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
                    app.Title = "VenusBuggy";
                    app.Width = Width;   
                    app.Height = Height;
                    app.WindowBorder = Border;     
                    app.WindowState = Fullscreen;

                    //tex_bg = game.createTexture("tex_bg", "texturen/MenuBG_1920_1080.jpg");
                    //tex_end = game.createTexture("tex_end", "texturen/MenuEnd.jpg");

                    //tex_bg = game.loadTexture("texturen/MenuBG_1920_1080.jpg");

                    tex_end = game.loadTexture("texturen/MenuEnd.png");
                    tex_bg = game.loadTexture("texturen/MenuBG_1920_1080.jpg");

                    //Bitmap MenuBG = new Bitmap("texturen/MenuBG_1920_1080.jpg");
                    //tex_bg = game.loadTexture(MenuBG);
                    //Bitmap MenuEnd = new Bitmap("texturen/MenuEnd.png");
                    //tex_end = game.loadTexture(MenuEnd);


                    //tex_end = game.loadTexture("texturen/MenuEnd.png");

                    //Console.WriteLine(tex_bg);
                    //Console.WriteLine(tex_end);


                    GL.Enable(EnableCap.Texture2D); //Texturierung aktivieren
                    GL.Enable(EnableCap.Blend); //Alpha-Kanäle aktivieren
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                    BGColor = Color.FromArgb(0, 128, 128, 128); //Die Standardfensterfarbe //Nur zur Sicherheit

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

                            game.drawMenu(Width, Height, tex_bg, tex_end);
                            break;
                    }





                    app.SwapBuffers();
                };

                app.Run(60.0);  //Updatefrequenz - Drawing wird notfalls übersprungen
            }
        }


        private void drawMenu(int Width, int Height, int bg, int end)
        {

            GL.BindTexture(TextureTarget.Texture2D, bg);
            GL.Begin(PrimitiveType.Quads);

                GL.TexCoord2(0, 0);
                GL.Vertex2(000.0f, Height);
                GL.TexCoord2(1, 0);
                GL.Vertex2(Width, Height);
                GL.TexCoord2(1, 1);
                GL.Vertex2(Width, 00.0f);
                GL.TexCoord2(0, 1);
                GL.Vertex2(00.0f, 00.0f);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, end);
            GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 1);
                GL.Vertex2(100.0f, 100.0f);
                GL.TexCoord2(1, 1);
                GL.Vertex2(325.0f, 100.0f);
                GL.TexCoord2(1, 0);
                GL.Vertex2(325.0f, 144.0f);
                GL.TexCoord2(0, 0);
                GL.Vertex2(100.0f, 144.0f);
            GL.End();

        }

        private int loadTexture(string filename)    //Fertiger Texturenlader
        {
            int id = GL.GenTexture();       //Nimm die Textur und gib ihr eine ID
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(filename);
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
    }
}
