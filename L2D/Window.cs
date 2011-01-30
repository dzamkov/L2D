using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using L2D.Engine;

namespace L2D
{
    public class Window : GameWindow
    {
        public Window()
            : base(640, 480, GraphicsMode.Default, "L2D")
        {
            Path resources = Path.ProcessFile.Parent.Parent.Parent.Parent["Resources"];
            Path shaders = resources["Shaders"];

            this._World = new World();
            this._World.Add(Atmosphere.MakeEntity(shaders, AtmosphereOptions.DefaultEarth, AtmosphereQualityOptions.Default));
        }

        /// <summary>
        /// Program main entry point.
        /// </summary>
        public static void Main(string[] Args)
        {
            new Window().Run(60.0);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(Color.RGB(0.0, 0.7, 1.0));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, (float)this.Width / (float)this.Height, 0.2f, 1000.0f);

            this._World.Visual.Render(ref proj, 0.2f, 1000.0f, this._PlayerPos, this.Foward);
            

            this.SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        /// <summary>
        /// Gets the direction the player is currently looking in.
        /// </summary>
        public Vector Foward
        {
            get
            {
                double cosx = Math.Cos(this._LookX);
                return new Vector(Math.Sin(this._LookZ) * cosx, Math.Cos(this._LookZ) * cosx, Math.Sin(this._LookX));
            }
        }

        private World _World;

        private Vector _PlayerPos;
        private double _LookX;
        private double _LookZ;
    }
}