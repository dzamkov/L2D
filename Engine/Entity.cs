using System;
using System.Collections.Generic;

namespace L2D.Engine
{
    /// <summary>
    /// An actor in a world whose properties are defined by components. Components inside the same entity may interact directly by
    /// sharing variables or values.
    /// </summary>
    public abstract class Entity
    {
        public Entity()
        {

        }

        /// <summary>
        /// An entity that contains a static collection of components.
        /// </summary>
        private class _SimpleEntity : Entity
        {
            public _SimpleEntity(IEnumerable<Component> Components)
            {
                this._Components = Components;
            }

            public override IEnumerable<Component> Components
            {
                get
                {
                    return this._Components;
                }
            }

            private IEnumerable<Component> _Components;
        }

        /// <summary>
        /// Creates a simple entity with the given components.
        /// </summary>
        public static Entity WithComponents(IEnumerable<Component> Components)
        {
            return new _SimpleEntity(Components);
        }

        /// <summary>
        /// Creates a simple entity with one components.
        /// </summary>
        public static Entity WithComponent(Component Component)
        {
            return new _SimpleEntity(new Component[] { Component });
        }

        /// <summary>
        /// Gets if the given entity is simple, and does not require an update.
        /// </summary>
        public static bool IsSimple(Entity Entity)
        {
            return Entity is _SimpleEntity;
        }

        /// <summary>
        /// Gets if the entity has been removed.
        /// </summary>
        public bool Removed
        {
            get
            {
                return this._Removed;
            }
        }

        /// <summary>
        /// Gets all the components in the entity.
        /// </summary>
        public abstract IEnumerable<Component> Components { get; }

        /// <summary>
        /// Gets a component of the specified type from the entity, if the entity defines it.
        /// </summary>
        public T GetComponent<T>()
            where T : Component
        {
            foreach (Component c in this.Components)
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
        /// Removes the entity from all associated worlds.
        /// </summary>
        public void Remove()
        {
            this._Removed = true;
            this.OnRemove();
            foreach (Component c in this.Components)
            {
                c._Removed = true;
                c.OnRemove(this);
            }
        }

        /// <summary>
        /// Called when the entity is removed. Components will be freed automatically.
        /// </summary>
        protected virtual void OnRemove()
        {

        }

        /// <summary>
        /// Called every tick to update the components against each other and possibly advance the state of the entity. No 
        /// inter-entity interactions may occur in an update.
        /// </summary>
        protected internal virtual void OnUpdate(double Time)
        {
            
        }

        /// <summary>
        /// Draw stuff or whatever, Called every frame.
        /// </summary>
        protected internal virtual void Draw()
        {
        }

        private bool _Removed;
    }

    /// <summary>
    /// Creates an entity based on a parameter.
    /// </summary>
    public delegate Entity EntityConstructor<T>(T Parameter);
}