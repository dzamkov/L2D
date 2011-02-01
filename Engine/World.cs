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
            this._TimeSystem = new TimeSystem();
            this._Entities = new LinkedList<Entity>();
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
        /// Gets the time subsystem.
        /// </summary>
        public TimeSystem Time
        {
            get
            {
                return this._TimeSystem;
            }
        }

        /// <summary>
        /// Adds an entity to the world.
        /// </summary>
        public void Add(Entity Entity)
        {
            foreach (Component c in Entity.Components)
            {
                VisualComponent vc = c as VisualComponent;
                if (vc != null)
                {
                    this._VisualSystem.Add(vc);
                }

                TimeComponent tc = c as TimeComponent;
                if (tc != null)
                {
                    this._TimeSystem.Add(tc);
                }
            }

            // Processing simple entities is a waste of time anyway.
            if (!Entity.IsSimple(Entity))
            {
                this._Entities.AddLast(Entity);
            }
        }

        /// <summary>
        /// Updates all entities and systems by the given time in seconds.
        /// </summary>
        public void Update(double Time)
        {
            // Update systems
            this._VisualSystem.Update(Time);
            this._TimeSystem.Update(Time);

            // Update entities
            LinkedListNode<Entity> cur = this._Entities.First;
            while (cur != null)
            {
                Entity ent = cur.Value;
                if (ent.Removed)
                {
                    LinkedListNode<Entity> next = cur.Next;
                    this._Entities.Remove(cur);
                    cur = next;
                }
                else
                {
                    ent.OnUpdate(Time);
                    cur = cur.Next;
                }
            }
        }

        private LinkedList<Entity> _Entities;
        private VisualSystem _VisualSystem;
        private TimeSystem _TimeSystem;
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

        /// <summary>
        /// Updates the system by the given time.
        /// </summary>
        public virtual void Update(double Time)
        {

        }
    }
}