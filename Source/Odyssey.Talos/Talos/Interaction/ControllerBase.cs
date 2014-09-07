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
using Odyssey.Engine;
using Odyssey.Talos.Components;
using SharpDX;

#endregion

namespace Odyssey.Talos.Interaction
{
    public abstract class ControllerBase : IEntityController
    {
        private readonly IServiceRegistry services;
        private PositionComponent cPosition;
        private RotationComponent cRotation;
        private UpdateComponent cUpdate;

        public ControllerBase(IServiceRegistry services)
        {
            this.services = services;
        }

        protected RotationComponent CRotation
        {
            get { return cRotation; }
        }

        protected UpdateComponent CUpdate
        {
            get { return cUpdate; }
        }

        protected PositionComponent CPosition
        {
            get { return cPosition; }
        }

        protected Entity Source { get; private set; }

        protected IServiceRegistry Services
        {
            get { return services; }
        }

        public virtual void BindToEntity(Entity source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            Source = source;
            if (!Source.TryGetComponent(out cPosition))
                throw new InvalidOperationException(string.Format("'{0}' does not contain a {1}", source.Name, cPosition.GetType()));
            if (!source.TryGetComponent(out cRotation))
                throw new InvalidOperationException(string.Format("'{0}' does not contain a {1}", source.Name, cRotation.GetType()));
            if (!source.TryGetComponent(out cUpdate))
                throw new InvalidOperationException(string.Format("'{0}' does not contain a {1}", source.Name, cUpdate.GetType()));
        }

        public abstract void Update(ITimeService time);
    }
}