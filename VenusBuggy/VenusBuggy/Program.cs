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

            //--------- Systemvariablen ----------//
            int cron = 0;   //Globale Steuervariable

            short Width = 1920; //Fenstergröße
            short Height = 1080;

            var BGColor = new Color(); //Hintergrundfarbe       BGColor = Color.FromArgb(0, 128, 128, 128);

            var Border = WindowBorder.Hidden;   //Rahmen
            var Fullscreen = WindowState.Fullscreen; //Vollbildmodus

            Point mousePos = new Point();

            //---------- Panels ----------//

            Panel pan_MenuBG = null;
            Panel pan_MenuEnd = null;
            Panel pan_MenuOpts = null;
            Panel pan_MenuNew = null;
            //Panel pan_MenuTest = null;
            
            using (var app = new GameWindow())
            {
                app.Load += (sender, e) =>
                {
                    app.Title = "VenusBuggy";
                    app.Width = Width;   
                    app.Height = Height;
                    app.WindowBorder = Border;     
                    app.WindowState = Fullscreen;

                    pan_MenuBG = new Panel(0, 0, Width, Height, "texturen/MenuBG_1920_1080.jpg");
                    pan_MenuEnd = new Panel(100, 100, 225, 44, "texturen/MenuEnd0.bmp", "texturen/MenuEnd1.bmp");
                    pan_MenuOpts = new Panel(100, 160, 225, 44, "texturen/MenuOpts0.bmp", "texturen/MenuOpts1.bmp");
                    pan_MenuNew = new Panel(100, 220, 225, 44, "texturen/MenuNew0.bmp", "texturen/MenuNew1.bmp");
                    //pan_MenuTest = new Panel(100, 160, 225, 44, "texturen/Bastelkopie.png");

                    GL.Enable(EnableCap.Texture2D); //Texturierung aktivieren
                    GL.Enable(EnableCap.Blend); //Alpha-Kanäle aktivieren


                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                    GL.Disable(EnableCap.DepthTest);

                    BGColor = Color.FromArgb(0, 128, 128, 128); //Die Standardfensterfarbe //Nur zur Sicherheit

                    app.VSync = VSyncMode.On;
                };

                app.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, app.Width, app.Height);
                };

                app.UpdateFrame += (sender, e) =>
                {
                    mousePos.X = app.Mouse.X;               //Aktualisiere Maus-Koordinaten
                    mousePos.Y = Height - app.Mouse.Y - 1;

                    if (app.Keyboard[Key.Escape])
                    {
                        app.Exit();
                    }

                    //##################################################################### Hier nächste Baustelle




                };

                app.RenderFrame += (sender, e) =>
                {
                    //Console.WriteLine(mousePos.X);
                    //Console.WriteLine(mousePos.Y);


                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();
                    GL.Ortho(0, app.Width, 0, app.Height, 0.0, 4.0);  //Nullpunkt ist unten links!

                    GL.ClearColor(BGColor);

                    switch (cron)
                    {
                        case 0: // 0 = Du befindest dich derzeit im Hauptmenü oberster Ebene

                            pan_MenuBG.draw();
                            pan_MenuEnd.draw();
                            pan_MenuOpts.draw();
                            pan_MenuNew.draw();
                            //pan_MenuTest.draw();
                            break;
                    }

                    app.SwapBuffers();
                };

                app.Run(60.0);  //Updatefrequenz - Drawing wird notfalls übersprungen
            }
        }
    }
}
