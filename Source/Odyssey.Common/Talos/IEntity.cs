#region License

// Copyright © 2013-2014 Avengers UTD - Adalberto L. Simeone
// 
// The Odyssey Engine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License Version 3 as published by
// the Free Software Foundation.
// 
// The Odyssey Engine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details at http://gplv3.fsf.org/

#endregion

#region Using Directives

using System;
using System.Collections.Generic;
using Odyssey.Content;

#endregion

namespace Odyssey.Talos
{
    public interface IEntity : IResource
    {
        /// <summary>
        /// Returns the sequential Id number of this instance.
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Returns a value indicating the combination of components registered to this instance.
        /// </summary>
        long Key { get; }

        bool IsEnabled { get; set; }

        IEnumerable<IComponent> Components { get; }

        bool ContainsComponent<TComponent>()
            where TComponent : IComponent;

        IEnumerable<string> Tags { get; }
        bool ContainsTag(string tag);

    }
}