using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace L2D.Engine
{
    /// <summary>
    /// A component in the visual system.
    /// </summary>
    public class VisualComponent : Component
    {

    }

    /// <summary>
    /// Handles the interactions of visual components.
    /// </summary>
    public class VisualSystem : System<VisualComponent>
    {
        public override void Add(VisualComponent Component)
        {
            // Is it an atmosphere (special case?)
            AtmosphereVisualComponent avc = Component as AtmosphereVisualComponent;
            if (avc != null)
            {
                this._Atmosphere = avc;
            }

            // Or a sun?
            SunVisualComponent svc = Component as SunVisualComponent;
            if (svc != null)
            {
                this._Sun = svc;
            }
        }

        /// <summary>
        /// Renders the contents of the visual system to the current GL context.
        /// </summary>
        public void Render(ref Matrix4 Proj, double Near, double Far, Vector EyePos, Vector EyeDir)
        {
            if (this._Sun != null && this._Sun.Removed) this._Sun = null;
            if (this._Atmosphere != null && this._Atmosphere.Removed) this._Atmosphere = null;


            // Render atmosphere
            if (this._Atmosphere != null)
            {
                if (this._Sun != null)
                {
                    this._Atmosphere.Render(this._Sun.Sun.Direction, ref Proj, Near, Far, EyePos, EyeDir);
                }
            }
        }

        private SunVisualComponent _Sun;
        private AtmosphereVisualComponent _Atmosphere;
    }

}