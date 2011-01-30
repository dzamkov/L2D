using System;
using System.Collections.Generic;

namespace L2D.Engine
{
    /// <summary>
    /// A dynamic collection of entities and systems.
    /// </summary>
    public class World
    {
        public World()
        {
            this._VisualSystem = new VisualSystem();
        }

        /// <summary>
        /// Gets the visual subsystem.
        /// </summary>
        public VisualSystem Visual
        {
            get
            {
                return this._VisualSystem;
            }
        }

        /// <summary>
        /// Links and adds an entity to the world.
        /// </summary>
        public void Add(Entity Entity)
        {
            Entity.Link();
            foreach (Component c in Entity.Components)
            {
                VisualComponent vc = c as VisualComponent;
                if (vc != null)
                {
                    this._VisualSystem.Add(vc);
                }
            }
        }

        private VisualSystem _VisualSystem;
    }

    /// <summary>
    /// Handles the interactions between components of a certain type.
    /// </summary>
    public abstract class System<T>
        where T : Component
    {
        /// <summary>
        /// Adds a component to the system, the component is removed when its Removed property is set.
        /// </summary>
        public abstract void Add(T Component);
    }
}