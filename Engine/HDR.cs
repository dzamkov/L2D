﻿using System;
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
        }

        /// <summary>
        /// Deletes all resources the HDR effect takes.
        /// </summary>
        public void Delete()
        {
            this._HDRTexture.Delete();
            GL.DeleteFramebuffers(1, ref this._FBO);
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

            GL.LoadIdentity();

            // Find bloom texture
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, Framebuffer);
            GL.Viewport(0, 0, this._Width, this._Height);
            

            // Final
            normal.Call();
            this._HDRTexture.SetUnit(TextureTarget.Texture2D, TextureUnit.Texture0);
            normal.SetUniform("Exposure", 1.0f);
            normal.SetUniform("Source", TextureUnit.Texture0);
            normal.DrawFull();
        }

        private double _Exposure;
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
            this.Blur = Blur;
        }

        public Blur Blur;
        public Shader Normal;
    }
}