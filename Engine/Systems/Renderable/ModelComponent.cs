using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;
using Jitter.Dynamics.Constraints;

namespace L2D.Engine
{
    /// <summary>
    /// blahblahtodo
    /// </summary>
    public class ModelComponent : VisualComponent
    {
        public ModelComponent(Model Model, ITransform Transform, Texture Texture)
        {
            this.Transform = Transform;
            this.Model = Model;
            this.Texture = Texture;
        }

        public void Render(Shader LightShader)
        {
            if (this.Model != null)
            {
                if (this.Texture != null)
                {
                    this.Texture.SetUnit(TextureTarget.Texture2D,
                        TextureUnit.Texture0);
                    LightShader.SetUniform("Material", TextureUnit.Texture0);
                }
                this.Model.Draw(Transform);
            }
        }

        public ITransform Transform { get; set; }
        public Model Model { get; set; }
        public Texture Texture { get; set; }
    }

    public interface ITransform
    {
        Vector      Position { get; }
        Matrix4d    Orientation { get; }
        Vector      Scale { get; }
    }
}
