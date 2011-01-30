using System;
using System.Collections.Generic;

namespace L2D.Engine
{
    /// <summary>
    /// An actor in a world whose properties are defined by components. Components inside the same entity may interact directly by
    /// sharing variables or values. Note that some combination of components may be automatically recognized and used as a combination by a world.
    /// </summary>
    public sealed class Entity
    {
        public Entity()
        {
            this._Components = new List<Component>();
        }

        /// <summary>
        /// Gets all the components in the entity.
        /// </summary>
        public IEnumerable<Component> Components
        {
            get
            {
                return this._Components;
            }
        }

        /// <summary>
        /// Gets a component of the specified type from the entity, if the entity defines it.
        /// </summary>
        public T GetComponent<T>()
            where T : Component
        {
            foreach (Component c in this._Components)
            {
                T tc = c as T;
                if (tc != null)
                {
                    return tc;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a component to the entity.
        /// </summary>
        public void LinkComponent(Component Component)
        {
            this._Components.Add(Component);
        }

        /// <summary>
        /// Interlinks all components in the entity. After this is called, no more components may be added.
        /// </summary>
        public void Link()
        {
            foreach (Component c in this._Components)
            {
                c.OnLink(this);
            }
        }

        /// <summary>
        /// Removes the entity from all associated worlds.
        /// </summary>
        public void Remove()
        {
            foreach (Component c in this._Components)
            {
                c._Removed = true;
                c.OnDispose(this);
            }
        }

        private List<Component> _Components;
    }

    /// <summary>
    /// Creates an entity based on a parameter.
    /// </summary>
    public delegate Entity EntityConstructor<T>(T Parameter);
}