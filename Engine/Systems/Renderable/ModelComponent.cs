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
        public ModelComponent(Model Model, ITransform Transform)
        {
            this.Transform = Transform;
            this.Model = Model;
        }

        public void Render()
        {
            if (this.Model != null)
            {
                this.Model.Draw(Transform);
            }
        }

        public ITransform Transform { get; set; }
        public Model Model { get; set; }
    }

    public interface ITransform
    {
        Vector      Position { get; }
        Angle       Orientation { get; }
        Vector      Scale { get; }
    }
}
