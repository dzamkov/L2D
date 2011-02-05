using System;
using System.Collections.Generic;

namespace L2D.Engine
{
    public class Angle
    {
        public Angle()
        {
            this._Pitch = 0.0;
            this._Yaw = 0.0;
            this._Roll = 0.0;
        }

        public Angle(double Pitch, double Yaw, double Roll)
        {
            this._Pitch = Pitch;
            this._Yaw = Yaw;
            this._Roll = Roll;
        }

        public static implicit operator Vector(Angle Ang)
        {
            return new Vector(Ang.Pitch, Ang.Yaw, Ang.Roll);
        }

        public double Pitch
        {
            get
            {
                return this._Pitch;
            }
            set
            {
                this._Pitch = value;
            }
        }
        public double Yaw
        {
            get
            {
                return this._Yaw;
            }
            set
            {
                this._Yaw = value;
            }
        }
        public double Roll
        {
            get
            {
                return this._Roll;
            }
            set
            {
                this._Roll = value;
            }
        }

        private double _Pitch;
        private double _Yaw;
        private double _Roll;
    }
}
