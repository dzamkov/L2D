//#define SHADERS
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;



using L2D.Engine;
using Color = L2D.Engine.Color;

namespace L2D
{
    public class Window : GameWindow
    {
        public Window()
            : base(640, 480, GraphicsMode.Default, "L2D")
        {
            Path resources = Path.ProcessFile.Parent.Parent.Parent.Parent["Resources"];
            Path shaders = resources["Shaders"];

            this.VSync = VSyncMode.Off;

            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.VertexArray);
            //GL.Enable(EnableCap.NormalArray);
            GL.Enable(EnableCap.TextureCoordArray);
			
			
            //GL.Enable(EnableCap.CullFace);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            this.WindowState = WindowState.Maximized;

            Blur b = new Blur(shaders);
            VisualSystem vissys = new VisualSystem(shaders);
            TimeSystem tsys = new TimeSystem();

            PhysicsSystem psys = new PhysicsSystem();
            this._World = new World(vissys, tsys, psys);

            

            // Make it midday
            tsys.Offset = tsys.SecondsPerDay / 2.0;
#if SHADERS
            this._World.Add(Atmosphere.MakeEntity(shaders, AtmosphereOptions.DefaultEarth, AtmosphereQualityOptions.Default));
			this._World.Add(new Sun(37.3 * Math.PI / 180.0 /* LOL my house */));
#endif
            this._World.Add(new GroundTest(resources, new Vector2d(1.0, 1.0)));
            this._World.Add(new PhysDuck(resources));
            
            vissys.Setup();
            vissys.SetSize(this.Width, this.Height);
 
			this._World.Add(this._Player = new Player(this._World.Physics.PhysWorld));
			
			this.Keyboard.KeyDown += delegate(object sender, KeyboardKeyEventArgs e)
            {
                if (e.Key == Key.Q) this._TimeRate *= 2.0;
                if (e.Key == Key.E) this._TimeRate *= 0.5;
            };
            this._TimeRate = 1.0;
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
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            Vector eyepos = this._Player.EyePosition;

            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, (float)this.Width / (float)this.Height, 0.2f, 1000.0f);
            Matrix4 view = Matrix4.LookAt(
                (Vector3)this._Player.EyePosition,
                (Vector3)(this._Player.EyePosition + this._Player.LookDirection),
                (Vector3)Vector.Up);
            
            this._World.Visual.Render(ref proj, 0.2f, 1000.0f, this._Player.EyePosition, this._Player.LookDirection);
            this._World.Draw();

            this.SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            this._World.Update(e.Time * this._TimeRate);

            this.Title = "L2D(" + ((int)this.RenderFrequency).ToString() + ")";
            // Mouse look
            double deltax = 0.0;
            double deltaz = 0.0;
            if (this.Focused)
            {
                Cursor.Hide();

                Point curpoint = Cursor.Position;
                Size screensize = Screen.PrimaryScreen.WorkingArea.Size;
                Point center = new Point(screensize.Width / 2, screensize.Height / 2);
                Cursor.Position = center;

                double mouserate = 0.001;
                deltaz += (double)(curpoint.X - center.X) * mouserate;
                deltax += (double)(curpoint.Y - center.Y) * -mouserate;
            }
            else
            {
                Cursor.Show();
            }

            double foward = 0.0;
            double side = 0.0;
            if (this.Keyboard[Key.W]) foward = 1.0;
            if (this.Keyboard[Key.S]) foward = -1.0;
            if (this.Keyboard[Key.A]) foward = -1.0;
            if (this.Keyboard[Key.D]) foward = 1.0;

            this._Player.UpdateControl(deltax, deltaz, foward, side, this.Keyboard);

            if (this.Keyboard[Key.Escape])
            {
                this.Close();
            }
        }

        protected override void OnResize(EventArgs e)
        {
#if !NO_SHADER
            this._World.Visual.SetSize(this.Width, this.Height);
#endif
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        private double _TimeRate;
        private World _World;
        private Player _Player;
    }
}