using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
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

            int Width = 0;
            int Height = 0;

            Point mousePos = new Point();
            var mouse = Mouse.GetState();
            bool globalClickLock = false;   //wenn ein Klick fortbesteht, so sperre via ClickLock, damit beim Drag auf einen Button dieser nicht aktiviert werden kann.

            var BGColor = new Color(); //Hintergrundfarbe       BGColor = Color.FromArgb(0, 128, 128, 128);

            var Border = WindowBorder.Hidden;   //Rahmen
            var Fullscreen = WindowState.Fullscreen; //Vollbildmodus

            Config config = new Config();

            int volumeMusic = config.getValue(1);
            int volumeEffects = config.getValue(2);

            foreach (DisplayIndex index in Enum.GetValues(typeof(DisplayIndex)))    //Nimm die derzeit besten Vollbildkoordinaten vom Primären Bildschirm
            {
                DisplayDevice device = DisplayDevice.GetDisplay(index);
                if (device == null)
                {
                    continue;
                }

                Width = device.Width;
                Height = device.Height;
            }

            //---------- Audios ----------// // Muss noch bearbeitet werden. Wahrscheinlich werden alle Daten in Wave-Instanzen gespeichert.

            Audio audio = new Audio();

            AudioContext context = new AudioContext();



            
                    


            //---------- Panels ----------//

            Panel pan_MenuBG = null;
            Panel pan_MenuEnd = null;
            Panel pan_MenuOpts = null;
            Panel pan_MenuNew = null;

            Panel pan_OptsBack = null;
            Panel lab_Music = null;
            Panel lab_Effects = null;
            HSlider hsl_VolumeMusic = null;
            HSlider hsl_VolumeEffects = null;
            //Panel pan_MenuTest = null;
            
            using (var app = new GameWindow())
            {
                app.Load += (sender, e) =>
                {
                    //Width = app.
                    //Height = app.Bounds.Y;

                    app.Title = "VenusBuggy";
                    app.Width = Width;
                    app.Height = Height;
                    app.WindowBorder = Border;
                    app.WindowState = Fullscreen;

                    pan_MenuBG = new Panel(0, 0, Width, Height, 0, "texturen/Menu/MenuBG_1920_1080.jpg");
                    pan_MenuEnd = new Panel(100, 100, 225, 44, -1, "texturen/Menu/MenuEnd0.bmp", "texturen/Menu/MenuEnd1.bmp");
                    pan_MenuOpts = new Panel(100, 160, 225, 44, 1, "texturen/Menu/MenuOpts0.bmp", "texturen/Menu/MenuOpts1.bmp");
                    pan_MenuNew = new Panel(100, 220, 225, 44, 10, "texturen/Menu/MenuNew0.bmp", "texturen/Menu/MenuNew1.bmp");

                    lab_Music = new Panel((int)(Width/2) - 300, (int)(Height/2) + 100, 225, 44, 0, "texturen/Menu/Music.bmp");
                    lab_Effects = new Panel((int)(Width / 2) - 300, (int)(Height / 2) + 40, 225, 44, 0, "texturen/Menu/Effects.bmp");

                    pan_OptsBack = new Panel((int)(Width / 2) - 300, (int)(Height / 2) -80, 225, 44, 2, "texturen/Menu/OptsBack0.bmp", "texturen/Menu/OptsBack1.bmp");

                    hsl_VolumeMusic = new HSlider(volumeMusic, (int)(Width / 2), (int)(Height / 2) + 100, 300, 5, 44,  11, 44, 13, 44, "texturen/Menu/SliderEnd.bmp", "texturen/Menu/SliderBar.bmp", "texturen/Menu/Slider0.bmp", "texturen/Menu/Slider1.bmp", "texturen/Menu/Slider2.bmp");
                    hsl_VolumeEffects = new HSlider(volumeEffects, (int)(Width / 2), (int)(Height / 2) + 40, 300, 5, 44, 11, 44, 13, 44, "texturen/Menu/SliderEnd.bmp", "texturen/Menu/SliderBar.bmp", "texturen/Menu/Slider0.bmp", "texturen/Menu/Slider1.bmp", "texturen/Menu/Slider2.bmp");

                    GL.Enable(EnableCap.Texture2D); //Texturierung aktivieren
                    GL.Enable(EnableCap.Blend); //Alpha-Kanäle aktivieren


                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                    //GL.Disable(EnableCap.DepthTest);

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
                    mouse = Mouse.GetState();

                    //if (app.Keyboard[Key.Escape])
                    //{
                    //    app.Exit();
                    //}

                    switch (cron)
                    {
                        case (-1):
                            app.Exit();
                            break;
                        case 0:
                            //Console.WriteLine(mouse.GetType().ToString());
                            cron = pan_MenuEnd.clickCheck(mousePos.X, mousePos.Y, mouse, cron, globalClickLock);
                            cron = pan_MenuOpts.clickCheck(mousePos.X, mousePos.Y, mouse, cron, globalClickLock);
                            pan_MenuNew.clickCheck(mousePos.X, mousePos.Y, mouse, cron, globalClickLock);
                            break;
                        case 1:
                            cron = pan_OptsBack.clickCheck(mousePos.X, mousePos.Y, mouse, cron, globalClickLock);

                            if (!hsl_VolumeEffects.clickLock)
                            {
                                volumeMusic = hsl_VolumeMusic.clickCheck(mousePos.X, mousePos.Y, mouse, volumeMusic);
                            }
                            if (!hsl_VolumeMusic.clickLock)
                            {
                                volumeEffects = hsl_VolumeEffects.clickCheck(mousePos.X, mousePos.Y, mouse, volumeEffects);
                            }
                            break;
                        case 2:
                            config.writeConfig(volumeMusic.ToString(), volumeEffects.ToString());
                            cron = 0;
                            break;
                    }

                    globalClickLock = (mouse[MouseButton.Left]);    //Überprüft, ob die Maustaste gehalten wird


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
                            break;
                        case 1:
                            pan_MenuBG.draw();
                            pan_OptsBack.draw();
                            lab_Music.draw();
                            lab_Effects.draw();
                            hsl_VolumeMusic.draw();
                            hsl_VolumeEffects.draw();



                            //Console.WriteLine(volumeMusic);
                            //Console.WriteLine(volumeEffects);
                            break;
                    }

                    app.SwapBuffers();
                };

                app.Run(60.0);  //Updatefrequenz - Drawing wird notfalls übersprungen
            }
        }
    }
}
