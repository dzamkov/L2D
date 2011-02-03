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
    /// Controls the ents physics
    /// </summary>
	public class PhysicsComponent : Component
	{
		public PhysicsSystem System
		{
			get
			{
				return this._System;
			}
		}
		
		
		public RigidBody PhysMesh
		{
			get
			{
				return this._PhysMesh;
			}
			set
			{
				if(this._PhysMesh != null)
				{
					this.System._PhysWorld.RemoveBody(this._PhysMesh);
				}
				this._PhysMesh = value;
				this._System._PhysWorld.AddBody(this._PhysMesh);
			}
		}

        public void Draw()
        {
            RigidBody body = this._PhysMesh;
            Vector vec = body.Position;

            GL.Color4(1.0,0.0,0.0,1.0);
            GL.Begin(BeginMode.Lines);

            GL.Vertex3(vec);

            GL.End();
        }

		public void Free()
		{
			this.System._PhysWorld.RemoveBody(this._PhysMesh);
		}
		
		private RigidBody _PhysMesh;
		
		internal PhysicsSystem _System;
	}
	
	/// <summary>
    /// Handles the physics.
    /// </summary>
	public class PhysicsSystem : System<PhysicsComponent>
	{
		public PhysicsSystem()
		{
            this._CollisionSystem = new CollisionSystemSAP();
            this._PhysWorld = new Jitter.World(this._CollisionSystem);
            this._Components = new LinkedList<PhysicsComponent>();

            Shape groundshape = new BoxShape(new Vector(100, 100, 1));
            RigidBody ground = new RigidBody(groundshape);

            ground.Tag = Color.RGB(50,255,50);
            ground.IsStatic = true;

            PhysicsComponent pc = new PhysicsComponent();
            pc._System = this;
            pc.PhysMesh = ground;

            this.Add(pc);
		}
		
		public override void Add (PhysicsComponent Component)
		{
			Component._System = this;
            this._Components.AddLast(Component);
		}
		
		public override void Update (double Time)
		{
			this._PhysWorld.Step((float)Time, false);
		}

        /// <summary>
        /// Draw all components (physmeshes)
        /// </summary>
        public override void Draw()
        {
            LinkedListNode<PhysicsComponent> node = this._Components.First;
            while (node != null)
            {
                PhysicsComponent pc = node.Value;
                node = node.Next;

                if (pc.Removed)
                {
                    this._Components.Remove(pc);
                    pc.Free();
                    continue;
                }
                    
                pc.Draw();
            }
        }

		public Jitter.World PhysWorld
		{
			get
			{
				return this._PhysWorld;
			}
		}
        internal LinkedList<PhysicsComponent> _Components;
		internal CollisionSystem _CollisionSystem;
		internal Jitter.World _PhysWorld;
	}
}
