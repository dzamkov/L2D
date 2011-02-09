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
    /// A test ent
    /// </summary>
    public class GroundTest : Entity
    {
        public GroundTest(Path res, Vector2d Size)
        {
            Shape shape = new BoxShape(new Vector(Size.X, Size.Y, 1.0));
            RigidBody body = new RigidBody(shape);
            body.Mass = 99999f;
            body.IsStatic = true;
            body.Position = new Vector(0.0, 0.0, 0.0);
            this._Phys = new PhysicsComponent(body);
            this._Phys.Scale = new Vector(Size.X, Size.Y, 1.0);
            this._Duck = new ModelComponent(Model.LoadFile(res["Models"]["cube.obj"].PathString), this._Phys);
            this._Duck.Model.Color = Color.RGB(1.0, 1.0, 1.0);
        }

        public override IEnumerable<Component> Components
        {
            get
            {
                yield return this._Phys;
                yield return this._Duck;
            }
        }

        protected internal override void OnUpdate(double Time)
        {
            this._Phys.PhysMesh.Update();
        }

        private PhysicsComponent _Phys;
        private ModelComponent _Duck;
    }
}