using System;
using System.Collections.Generic;

namespace L2D.Engine
{
    /// <summary>
    /// A part of an entity that handles some type of input, output and interaction. Components of related types and duties may interact indirectly through 
    /// their interfaces, even if they do not belong to the same entity.
    /// </summary>
    public class Component
    {
        /// <summary>
        /// Gets if the component is to be removed. If this is true the component may no longer be used.
        /// </summary>
        public bool Removed
        {
            get
            {
                return this._Removed;
            }
        }

        /// <summary>
        /// Called when the component will no longer be needed.
        /// </summary>
        internal protected virtual void OnRemove(Entity Entity)
        {

        }

        internal bool _Removed;
    }
}