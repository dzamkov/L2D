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
        /// Called when a component is linked to an entity.
        /// </summary>
        internal protected virtual void OnLink(Entity Entity)
        {

        }
    }
}