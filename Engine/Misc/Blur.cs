using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace L2D.Engine
{
    /// <summary>
    /// Holds resources needed to gaussian blur a (5x5 texel area) using a shader.
    /// </summary>
    public class Blur
    {
        public Blur(Path Shaders)
        {
            Shader.PrecompilerInput pi = Shader.CreatePrecompilerInput();
            Path file = Shaders["Blur.glsl"];
            this._BlurHorizontal = Shader.Load(file, pi);
            pi.Define("VERTICAL");
            this._BlurVertical = Shader.Load(file, pi);
        }

        /// <summary>
        /// Deletes all resources used by the blur effect.
        /// </summary>
        public void Delete()
        {
            this._BlurHorizontal.Delete();
            this._BlurVertical.Delete();
        }

        /// <summary>
        /// Applies a blur effect to the source texture. This requires a temporary texture and an active frame buffer. The source and destination buffers can be the same.
        /// </summary>
        public void Apply(Texture Source, Texture Temp, Texture Dest, int Width, int Height)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, Temp.ID, 0);
            this._BlurHorizontal.Call();
            Source.SetUnit(TextureTarget.Texture2D, TextureUnit.Texture0);
            this._BlurHorizontal.SetUniform("Source", TextureUnit.Texture0);
            this._BlurHorizontal.SetUniform("PixelSpacing", 1.0f / (float)Width);
            this._BlurHorizontal.DrawFull();
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, Dest.ID, 0);
            this._BlurVertical.Call();
            Temp.SetUnit(TextureTarget.Texture2D, TextureUnit.Texture0);
            this._BlurVertical.SetUniform("Source", TextureUnit.Texture0);
            this._BlurVertical.SetUniform("PixelSpacing", 1.0f / (float)Height);
            this._BlurVertical.DrawFull();
        }

        private Shader _BlurHorizontal;
        private Shader _BlurVertical;
    }
}