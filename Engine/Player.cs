using System;
using System.Collections.Generic;

namespace L2D.Engine
{
    /// <summary>
    /// A controllable humanoid.
    /// </summary>
    public class Player : Entity
    {
        public Player()
        {

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
                return this._Position + new Vector(0.0, 0.0, 1.7);
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
        public void UpdateControl(double LookXDelta, double LookZDelta, double Foward, double Side)
        {
            this._LookX += LookXDelta;
            this._LookZ += LookZDelta;

            double quaterarc = Math.PI / 2.0;
            this._LookX = Math.Min(quaterarc * 0.9, Math.Max(-quaterarc * 0.9, this._LookX));
            this._LookZ = this._LookZ % (Math.PI * 2.0);
        }

        private Vector _Position;
        private Vector _Velocity;
        private double _LookX;
        private double _LookZ;
    }
}