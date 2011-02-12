#define SHADERS
using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace L2D.Engine
{
    /// <summary>
    /// A component in the visual system.
    /// </summary>
    public class VisualComponent : Component
    {

    }

    /// <summary>
    /// Handles the interactions of visual components.
    /// </summary>
    public class VisualSystem : System<VisualComponent>
    {
        public VisualSystem(Path Shaders)
        {
            this._HDRShaders = new HDRShaders(Shaders, new Blur(Shaders));
            this._CopyShader = Shader.Load(Shaders["Copy.glsl"]);
            this._LightingShader = Shader.Load(Shaders["Light.glsl"]);
            this._Models = new LinkedList<ModelComponent>();
        }

        public override void Add(VisualComponent Component)
        {
            // Is it an atmosphere (special case?)
            AtmosphereVisualComponent avc = Component as AtmosphereVisualComponent;
            if (avc != null)
            {
                this._Atmosphere = avc;
            }

            // Or a sun?
            SunVisualComponent svc = Component as SunVisualComponent;
            if (svc != null)
            {
                this._Sun = svc;
            }

            ModelComponent mvc = Component as ModelComponent;
            if (mvc != null)
            {
                this._Models.AddLast(mvc);
            }
        }

        /// <summary>
        /// Sets up the current graphics context for rendering.
        /// </summary>
        public void Setup()
        {
            
        }

        /// <summary>
        /// Sets the display size.
        /// </summary>
        public void SetSize(int Width, int Height)
        {
            if (this._HDR != null)
            {
                this._HDR.Delete();
            }
            this._HDR = new HDR(Width, Height, this._HDRShaders);
        }

        /// <summary>
        /// Renders the contents of the visual system to the current GL context.
        /// </summary>
        public void Render(ref Matrix4 Proj, double Near, double Far, Vector EyePos, Vector EyeDir)
        {
            GL.Disable(EnableCap.DepthTest);
#if SHADERS
            if (this._Sun != null && this._Sun.Removed) this._Sun = null;
            if (this._Atmosphere != null && this._Atmosphere.Removed) this._Atmosphere = null;

            // Start hdr
            if (this._HDR != null)
            {
                this._HDR.Start();
            }

            // Render atmosphere
            if (this._Atmosphere != null)
            {
                if (this._Sun != null)
                {
                    this._Atmosphere.Render(this._Sun.Sun.Direction, ref Proj, Near, Far, EyePos, EyeDir);
                }
            }

            

            GL.Enable(EnableCap.DepthTest);

            Matrix4 view = Matrix4.LookAt(
                (Vector3)EyePos,
                (Vector3)(EyePos + EyeDir),
                (Vector3)Vector.Up);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref Proj);
            GL.MultMatrix(ref view);// not depricated

            this._LightingShader.Call();
            this._LightingShader.SetUniform("SunDirection", this._Sun.Sun.Direction);
            this._LightingShader.SetUniform("Diffuse", 0.5f);
            this._LightingShader.SetUniform("Ambient", 0.5f);

            DrawModels(this._LightingShader);

            // End hdr
            if (this._HDR != null)
            {
                this._HDR.End(0);
            }
            Shader.Dismiss();
#endif

            
        }

        private void DrawModels(Shader LightShader)
        {
            LinkedListNode<ModelComponent> node = this._Models.First;
            
            while (node != null)
            {
                LinkedListNode<ModelComponent> nextnode = node.Next;
                ModelComponent m = node.Value;

                if (m.Removed)
                    this._Models.Remove(node);
                else
                {
                    m.Render(LightShader);
                }
                
                node = nextnode;
            }
        }

        private HDR _HDR;
        private HDRShaders _HDRShaders;
        private Shader _CopyShader;
        private Shader _LightingShader;
        private SunVisualComponent _Sun;
        private AtmosphereVisualComponent _Atmosphere;

        private LinkedList<ModelComponent> _Models;
    }

}