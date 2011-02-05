using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace L2D.Engine
{
    /// <summary>
    /// A big firey ball of gas that illuminates the world.
    /// </summary>
    public class Sun : Entity
    {
        public Sun(double Latitude)
            : this(Latitude, 0.409)
        {

        }

        public Sun(double Latitude, double Tilt)
        {
            this._Visual = new SunVisualComponent(this);
            this._Time = new TimeComponent();
            this._Latitude = Latitude;
            this._Tilt = Tilt;
        }

        /// <summary>
        /// Gets the direction of the sun where (0.0, 0.0, 1.0) is above the world. (0.0, 1.0, 0.0) is north.
        /// </summary>
        public Vector Direction
        {
            get
            {
                // http://www.gamedev.net/topic/582708-what-axis-should-a-directional-light-rotate-on-to-emulate-the-sun/

                const double circle = Math.PI * 2.0;
                double days = this._Time.System.Days * circle;
                double years = this._Time.System.Years * circle;

                double s = -Math.PI / 2.0 + this._Tilt * Math.Cos(years);
                double a = this._Latitude;
                Vector sunvec = new Vector(0.0, 0.0, -1.0);
                sunvec = sunvec.Rotate(new Vector(1.0, 0.0, 0.0), -s);
                sunvec = sunvec.Rotate(new Vector(0.0, 0.0, 1.0), days);
                sunvec = sunvec.Rotate(new Vector(1.0, 0.0, 0.0), -Math.PI / 2.0 + a);
                sunvec.Y = -sunvec.Y;
                sunvec.X = -sunvec.X;


                return sunvec;
            }
        }

        public override IEnumerable<Component> Components
        {
            get
            {
                yield return this._Visual;
                yield return this._Time;
            }
        }

        private SunVisualComponent _Visual;
        private TimeComponent _Time;
        private double _Latitude;
        private double _Tilt;
    }

    public class SunVisualComponent : VisualComponent
    {
        public SunVisualComponent(Sun Sun)
        {
            this._Sun = Sun;
        }

        /// <summary>
        /// Gets the sun this is for.
        /// </summary>
        public Sun Sun
        {
            get
            {
                return this._Sun;
            }
        }

        private Sun _Sun;
    }
}