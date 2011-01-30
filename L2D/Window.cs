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

            // Load an atmosphere
            Shader.PrecompilerInput pci = Shader.CreatePrecompilerInput();

            AtmosphereOptions options = AtmosphereOptions.DefaultEarth;
            AtmosphereQualityOptions qualityoptions = AtmosphereQualityOptions.Default;

            this._Atmosphere = Atmosphere.Generate(options, qualityoptions, pci, shaders);

            Path atmosphere = shaders["Atmosphere"];
            Path precompute = atmosphere["Precompute"];
            Atmosphere.DefineConstants(options, qualityoptions, pci);
            this._Planet = Shader.Load(atmosphere["Planet.glsl"], pci);
            this._PlanetRadius = options.RadiusGround;
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
            this._RenderAtmosphere(ref proj, 0.2, 1000.0, Vector.Normalize(new Vector(1.0, 0.0, 0.1)));
            

            this.SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        private void _RenderAtmosphere(ref Matrix4 Proj, double Near, double Far, Vector LookDirection)
        {
            GL.DepthFunc(DepthFunction.Lequal);
            GL.LoadIdentity();

            Vector eyepos = new Vector(0.0, 0.0, this._PlanetRadius);
            Matrix4 view = Matrix4.LookAt((Vector3)eyepos, (Vector3)(eyepos + LookDirection), new Vector3(0.0f, 0.0f, 1.0f));
            Matrix4 total = view * Proj;
            total.Invert();

            this._Atmosphere.Setup(this._Planet);
            this._Planet.SetUniform("EyePosition", eyepos);
            this._Planet.SetUniform("SunDirection", Vector.Normalize(new Vector(1.0, 0.0, 0.3)));
            this._Planet.SetUniform("ProjectionInverse", ref total);
            this._Planet.SetUniform("NearDistance", (float)Near);
            this._Planet.SetUniform("FarDistance", (float)Far);
            this._Planet.DrawFull();
        }

        /// <summary>
        /// Gets the direction the player is currently looking in.
        /// </summary>
        public Vector Foward
        {
            get
            {
                double sinx = Math.Sin(this._LookX);
                return new Vector(Math.Sin(this._LookZ) * sinx, Math.Cos(this._LookZ) * sinx, Math.Cos(this._LookX));
            }
        }

        private PrecomputedAtmosphere _Atmosphere;
        private Shader _Planet;
        private double _PlanetRadius;

        private Vector _PlayerPos;
        private double _LookX;
        private double _LookZ;
    }
}