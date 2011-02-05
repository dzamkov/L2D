using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace L2D.Engine
{
    /// <summary>
    /// A component in the time system. Can receive update of what the global time is.
    /// </summary>
    public class TimeComponent : Component
    {
        /// <summary>
        /// Gets the current time in the world this was added in.
        /// </summary>
        public double Time
        {
            get
            {
                if (this._System == null)
                {
                    return 0.0;
                }
                return this._System.Time;
            }
        }

        /// <summary>
        /// Gets the time system associated with this component.
        /// </summary>
        public TimeSystem System
        {
            get
            {
                return this._System;
            }
        }

        internal TimeSystem _System;
    }

    /// <summary>
    /// Handles the passage of time and reports it to time components.
    /// </summary>
    public class TimeSystem : System<TimeComponent>
    {
        public TimeSystem()
        {
            this._SecondsPerDay = 24.0 * 60.0 * 60.0;
            this._DaysPerYear = 365.24;
        }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        public double Time
        {
            get
            {
                return this._Time;
            }
        }

        /// <summary>
        /// Gets the current day where a number ending in .0 indicates midnight and a number ending in .5 indicates noon.
        /// </summary>
        public double Days
        {
            get
            {
                return (this.Time + this.Offset) / this._SecondsPerDay;
            }
        }

        /// <summary>
        /// Gets the current year where a number ending in .0 indicates winter (in the northern hemisphere) and a number ending in .5 indicates summer.
        /// </summary>
        public double Years
        {
            get
            {
                return this.Days / this._DaysPerYear;
            }
        }

        /// <summary>
        /// Gets or sets the offset of time from the midnight of the first day in seconds.
        /// </summary>
        public double Offset
        {
            get
            {
                return this._Offset;
            }
            set
            {
                this._Offset = value;
            }
        }

        /// <summary>
        /// Gets or sets the length of a day.
        /// </summary>
        public double SecondsPerDay
        {
            get
            {
                return this._SecondsPerDay;
            }
            set
            {
                this._SecondsPerDay = value;
            }
        }

        /// <summary>
        /// Gets or sets the length of a year.
        /// </summary>
        public double DaysPerYear
        {
            get
            {
                return this._DaysPerYear;
            }
            set
            {
                this._DaysPerYear = value;
            }
        }

        public override void Add(TimeComponent Component)
        {
            Component._System = this;
        }

        public override void Update(double Time)
        {
            this._Time += Time;
        }

        private double _Time;
        private double _Offset;
        private double _SecondsPerDay;
        private double _DaysPerYear;
    }
}