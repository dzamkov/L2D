using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace L2D.Engine
{
    /// <summary>
    /// Handles the resources needed to do an HDR rendering (possibly with bloom).
    /// </summary>
    public class HDR
    {
        public HDR(int Width, int Height, HDRShaders Shaders)
        {
            this._Width = Width;
            this._Height = Height;

            GL.GenFramebuffers(1, out this._FBO);
            this._HDRTexture = Texture.Initialize2D(Width, Height, Texture.RGB16Float);
            this._HDRTexture.SetInterpolation2D(TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            this._Shaders = Shaders;

            this._Levels = new List<_BlurLevel>();
            int b = 0;
            int nw, nh;
            while(_NextLevel(Width, Height, out nw, out nh) && b < 8)
            {
                Width = nw;
                Height = nh;
                this._Levels.Add(new _BlurLevel(Width, Height));
                b++;
            }
        }

        /// <summary>
        /// Calculates the next size of the blur level. Returns false if no more levels are needed.
        /// </summary>
        private static bool _NextLevel(int Width, int Height, out int NextWidth, out int NextHeight)
        {
            NextWidth = Width / 4;
            NextHeight = Height / 4;
            if (Width < 8 && Height < 8)
            {
                return false;
            }
            else
            {
                if (NextWidth > NextHeight)
                {
                    NextHeight += NextWidth / 4;
                }
                if (NextWidth < NextHeight)
                {
                    NextWidth += NextHeight / 4;
                }
                return true;
            }
        }

        /// <summary>
        /// Deletes all resources the HDR effect takes.
        /// </summary>
        public void Delete()
        {
            this._HDRTexture.Delete();
            GL.DeleteFramebuffers(1, ref this._FBO);
            foreach (_BlurLevel level in this._Levels)
            {
                level.Main.Delete();
                level.Temp.Delete();
            }
        }

        /// <summary>
        /// Starts an HDR rendering. All future render calls will be done to a framebuffer that allows
        /// floating-point color components, and as such, would be best done by shaders.
        /// </summary>
        public void Start()
        {
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, this._FBO);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, this._HDRTexture.ID, 0);
            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
        }

        /// <summary>
        /// Stops an HDR rendering and displays all contents to the specified framebuffer (0 for default).
        /// </summary>
        public void End(uint Framebuffer)
        {
            Shader normal = this._Shaders.Normal;
            Shader copy = this._Shaders.Copy;
            Blur blur = this._Shaders.Blur;

            GL.LoadIdentity();

            // Begin downscaling
            Texture cur = this._HDRTexture;
            copy.SetUniform("Source", TextureUnit.Texture0);
            foreach (_BlurLevel level in this._Levels)
            {
                GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, level.Main.ID, 0);
                GL.Viewport(0, 0, level.Width, level.Height);
                cur.SetUnit(TextureTarget.Texture2D, TextureUnit.Texture0);
                copy.DrawFull();
                blur.Apply(level.Main, level.Temp, level.Main, level.Width, level.Height);
                cur = level.Main;
            }

            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, Framebuffer);
            GL.Viewport(0, 0, this._Width, this._Height);
            

            // Final
            normal.Call();

            // Link bloom textures
            double w = 0.1 / (double)this._Levels.Count;
            for (int t = 0; t < 8; t++)
            {
                double weight = 0.0;
                string tstr = t.ToString();
                if (t < this._Levels.Count)
                {
                    TextureUnit unit = (TextureUnit)((int)TextureUnit.Texture1 + t);
                    this._Levels[t].Main.SetUnit(TextureTarget.Texture2D, unit);
                    normal.SetUniform("Bloom" + tstr, unit);
                    weight = w;
                }
                normal.SetUniform("W" + tstr, (float)weight);
            }

            this._HDRTexture.SetUnit(TextureTarget.Texture2D, TextureUnit.Texture0);
            normal.SetUniform("Exposure", 1.0f);
            normal.SetUniform("Source", TextureUnit.Texture0);
            normal.DrawFull();
        }

        /// <summary>
        /// Level of blurring when computing the bloom texture.
        /// </summary>
        private class _BlurLevel
        {
            public _BlurLevel(int Width, int Height)
            {
                this.Width = Width;
                this.Height = Height;
                this.Main = Texture.Initialize2D(Width, Height, Texture.RGB16Float);
                this.Temp = Texture.Initialize2D(Width, Height, Texture.RGB16Float);
            }

            public int Width;
            public int Height;
            public Texture Main;
            public Texture Temp;
        }

        private double _Exposure;
        private List<_BlurLevel> _Levels;
        private int _Width;
        private int _Height;
        private int _FBO;
        private Texture _HDRTexture;
        private HDRShaders _Shaders;
    }

    /// <summary>
    /// Stores the shaders for an HDR effect.
    /// </summary>
    public struct HDRShaders
    {
        public HDRShaders(Path Shaders, Blur Blur)
        {
            this.Normal = Shader.Load(Shaders["HDR.glsl"]);
            this.Copy = Shader.Load(Shaders["Copy.glsl"]);
            this.Blur = Blur;
        }

        public Blur Blur;
        public Shader Copy;
        public Shader Normal;
    }
}