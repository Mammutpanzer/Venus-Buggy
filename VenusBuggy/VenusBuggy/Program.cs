﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
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



            using (var app = new GameWindow())
            {
                app.Load += (sender, e) =>
                {
                    app.Width = 1366;   //Fenstergröße
                    app.Height = 768;


                    GL.ClearColor(Color.AliceBlue);

                    app.WindowBorder = WindowBorder.Hidden;     //Rahmen
                    app.WindowState = WindowState.Fullscreen;   //Vollbild
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



                    GL.Begin(PrimitiveType.Triangles);
                    //GL.BindTexture(TextureTarget.Texture2D, test);
                    //GL.TexCoord2(0, 0);
                    GL.Color3(255, 128, 255);
                    GL.Vertex2(100.0f, 200.0f);
                    //GL.TexCoord2(1, 0);
                    GL.Vertex2(0.0f, 0.0f);
                    //GL.TexCoord2(0.5, 1);
                    GL.Vertex2(600.0f, 10.0f);

                    GL.End();

                    app.SwapBuffers();
                };

                app.Run(60.0);  //Updatefrequenz - Drawing wird notfalls übersprungen
            }
        }
    }
}
