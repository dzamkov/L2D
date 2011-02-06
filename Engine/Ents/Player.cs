using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;
using Jitter.Dynamics.Constraints;

namespace L2D.Engine
{
    /// <summary>
    /// A controllable humanoid.
    /// </summary>
    public class Player : Entity
    {
        public Player(Jitter.World World)
        {
            this.Position = new Vector(0.0, 0.0, 0.0);
            this._Body = new RigidBody(new BoxShape(0.5f, 1.7f, 0.5f));
            //this._Body.UseUserMassProperties(new JMatrix(), 1.0f);
            this._Body.Restitution = 0.0f;
            this._Body.Position = new Vector(0.0, 0.0, 0.0);
            //this._Controller = new CharacterController(World, this._Body);
            //this._Controller.Position = new Vector(0.0, 0.0, 10.0);

            this._Phys = new PhysicsComponent(this._Body);
            
            //World.AddConstraint(this._Controller);
        }

        public override IEnumerable<Component> Components
        {
            get
            {
                yield return this._Phys;
            }
        }

        /// <summary>
        /// Gets or sets the position of the feet of the player.
        /// </summary>
        public Vector Position
        {
            get
            {
                Vector ret = this._Position;
                ret.UnNaN();
                return ret;
            }
            set
            {
                this._Position = value;
            }
        }

        /// <summary>
        /// Gets the position of the eyes of the player.
        /// </summary>
        public Vector EyePosition
        {
            get
            {
                // Average eye level as reported by wikipedia.
                Vector ret = this.Position;
                ret = ret + new Vector(0.0, 0.0, 1.7);
                return ret;
            }
        }

        /// <summary>
        /// Gets the normalized direction in which the player is looking.
        /// </summary>
        public Vector LookDirection
        {
            get
            {
                double cosx = Math.Cos(this._LookX);
                return new Vector(Math.Sin(this._LookZ) * cosx, Math.Cos(this._LookZ) * cosx, Math.Sin(this._LookX));
            }
        }

        /// <summary>
        /// Updates the player with user control.
        /// </summary>
        public void UpdateControl(double LookXDelta, double LookZDelta, double Foward, double Side, KeyboardDevice Keys)
        {
            this._LookX += LookXDelta;
            this._LookZ += LookZDelta;

            double quaterarc = Math.PI / 2.0;
            this._LookX = Math.Min(quaterarc * 0.9, Math.Max(-quaterarc * 0.9, this._LookX));
            this._LookZ = this._LookZ % (Math.PI * 2.0);
            const double rad = 0.0174532925;
            Vector targvel = new Vector();

            Vector right = this.LookDirection;
            right.Z = 0.0;
            right.Normalize();
            right = right.Rotate(Vector.Up, -90.0 * rad);


            if (Keys[Key.W])
                targvel = targvel + this.LookDirection;
            if (Keys[Key.S])
                targvel = targvel - this.LookDirection;
            if (Keys[Key.D])
                targvel = targvel + right;
            if (Keys[Key.A])
                targvel = targvel - right;

            targvel.Normalize();

            if (targvel.Length > 0.0)
                this.Position += targvel * 0.1;
        }

        RigidBody _Body;
        PhysicsComponent _Phys;

        private Vector _Position;
        private double _LookX;
        private double _LookZ;
    }

}