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
            this._Body = new RigidBody(new CapsuleShape(1.0f, 0.5f));
            this._Body.UseUserMassProperties(new JMatrix(), 1.0f);
            this._Body.Restitution = 0.0f;
            
            this._Controller = new CharacterController(World, this._Body);
            this._Controller.Position = new Vector(0.0, 0.0, 1.5);
            World.AddBody(this._Body);
            World.AddConstraint(this._Controller);
        }

        public override IEnumerable<Component> Components
        {
            get
            {
                return new Component[0];
            }
        }

        /// <summary>
        /// Gets or sets the position of the feet of the player.
        /// </summary>
        public Vector Position
        {
            get
            {
                return this._Position;
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
                Vector ret = this._Controller.Body1.Position;
                ret = ret + new Vector(0.0, 0.0, 1.7);
                ret.UnNaN();
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

            Vector targvel = new Vector();

            if (Keys[Key.W])
                targvel = targvel + Vector.Forward;

            targvel = targvel * this.LookDirection;
            targvel.Normalize();

            this._Controller.TargetVelocity = targvel;
            //this._Controller.Position = new Vector(0.0, 0.0, 1.0);
            this.Position = this._Controller.Position;
        }

        RigidBody _Body;
        CharacterController _Controller;

        private Vector _Position;
        private Vector _Velocity;
        private double _LookX;
        private double _LookZ;
    }

    /// <summary>
    /// Taken from Jitter-Physics.com help section (I NOT LAZY)
    /// </summary>
    public class CharacterController : Constraint, ITransform
    {
        private const float JumpVelocity = 0.5f;

        private float feetPosition;

        public CharacterController(Jitter.World world, RigidBody body)
            : base(body, null)
        {
            this.World = world;

            JVector vec = Vector.Down;
            JVector result = new Vector();

            // Note: the following works just for normal shapes, for multishapes (compound for example)
            // you have to loop through all sub-shapes -> easy.
            body.Shape.SupportMapping(ref vec, out result);

            // feet position is now the distance between body.Position and the feets
            // Note: the following '*' is the dot product.
            feetPosition = result * new JVector();
        }

        public override void AddToDebugDrawList(List<JVector> lineList, List<JVector> pointList)
        {
            throw new NotImplementedException();
        }

        public Jitter.World World { private set; get; }
        public JVector TargetVelocity { get; set; }
        public bool TryJump { get; set; }
        public RigidBody BodyWalkingOn { get; set; }

        private JVector deltaVelocity = new Vector();
        private bool shouldIJump = false;

        public override void PrepareForIteration(float timestep)
        {
            // send a ray from our feet position down.
            // if we collide with something which is 0.05f units below our feets remember this!

            RigidBody resultingBody = null;
            JVector normal; float frac;

            bool result = World.CollisionSystem.Raycast(Body1.Position + JVector.Down * (feetPosition - 0.1f), Vector.Down, RaycastCallback,
                out resultingBody, out normal, out frac);

            BodyWalkingOn = (result && frac <= 0.2f) ? resultingBody : null;
            shouldIJump = (result && frac <= 0.2f && Body1.LinearVelocity.Y < JumpVelocity && TryJump);
        }

        private bool RaycastCallback(RigidBody body, JVector normal, float fraction)
        {
            // prevent the ray to collide with ourself!
            return (body != this.Body1);
        }

        public Vector Position
        {
            get
            {
                Vector ret = this.Body1.Position;
                ret.UnNaN();
                return ret;
            }
            set
            {
                this.Body1.Position = value;
            }
        }
        public Angle Orientation { get; set; }
        public Vector Scale { get { return new Vector(1.0, 1.0, 1.0); } }

        public override void Iterate()
        {
            deltaVelocity = TargetVelocity - Body1.LinearVelocity;
            deltaVelocity.Y = 0.0f;

            // determine how 'stiff' the character follows the target velocity
            deltaVelocity *= 0.02f;

            if (deltaVelocity.LengthSquared() != 0.0f)
            {
                Body1.IsActive = true;
                Body1.ApplyImpulse(deltaVelocity * Body1.Mass);
            }

            if (shouldIJump)
            {
                Body1.IsActive = true;
                Body1.ApplyImpulse(JumpVelocity * JVector.Up * Body1.Mass);
                
                if (!BodyWalkingOn.IsStatic)
                {
                    BodyWalkingOn.IsActive = true;
                    BodyWalkingOn.ApplyImpulse(-1.0f * JumpVelocity * JVector.Up * Body1.Mass);
                }

            }
        }
    }
}