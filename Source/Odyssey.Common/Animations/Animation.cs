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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml;
using Odyssey.Content;
using Odyssey.Engine;
using Odyssey.Serialization;
using Odyssey.Utilities.Text;

#endregion

namespace Odyssey.Animations
{
    public delegate void CurveUpdateCallback(IAnimationCurve curve, object currentValue);
    public class Animation : ISerializableResource, IResource
    {
        private readonly Dictionary<string, IAnimationCurve> animationCurves;
        private float currentDuration;
        private float time;

        public AnimationStatus Status { get; private set; }

        public Animation()
        {
            animationCurves = new Dictionary<string, IAnimationCurve>();
            Speed = 1.0f;
        }

        public float Duration
        {
            get { return animationCurves.Values.Max(a => a.Duration); }
        }

        public float Time
        {
            get { return time; }
            set
            {
                time = value;
                NormalizedTime = time/Duration;
            }
        }

        public event EventHandler<EventArgs> Started;
        public event EventHandler<EventArgs> Completed;

        public float NormalizedTime { get; private set; }
        public WrapMode WrapMode { get; set; }
        public float Speed { get; set; }

        public IEnumerable<IAnimationCurve> Curves
        {
            get { return animationCurves.Values; }
        }

        public string Name { get; set; }

        [Pure]
        public bool Contains(IAnimationCurve animationCurve)
        {
            Contract.Requires<ArgumentNullException>(animationCurve != null, "animationCurve");
            return animationCurves.ContainsKey(animationCurve.Name);
        }

        public void AddCurve(IAnimationCurve animationCurve)
        {
            Contract.Requires<ArgumentException>(!Contains(animationCurve), "animationCurve");
            animationCurves.Add(animationCurve.Name, animationCurve);
        }

        public void RemoveCurve(string key)
        {
            animationCurves.Remove(key);
        }

        public IAnimationCurve this[int index]
        {
            get { return animationCurves.Values.ElementAt(index); }
        }

        public void Update(ITimeService time, CurveUpdateCallback updateCallback)
        {
            this.time += time.FrameTime;

            foreach (var curve in Curves)
            {
                var elapsedTime = Speed >= 0 ? this.time : currentDuration- this.time;
                var value = curve.Evaluate(elapsedTime);
                updateCallback(curve, value);
            }

            if (this.time > currentDuration)
            {
                Rewind();
                if (WrapMode == WrapMode.Once)
                    OnComplete(new EventArgs());
            }
        }

        public void Start()
        {
            currentDuration = Duration;
            Status = AnimationStatus.Playing;
            OnStarted(new EventArgs());
        }

        public void Stop()
        {
            Status = AnimationStatus.Stopped;
            OnComplete(new EventArgs());
        }

        public void Rewind()
        {
            time = 0;
        }

        protected virtual void OnComplete(EventArgs e)
        {
            RaiseEvent(Completed, this, e);
        }

        protected virtual void OnStarted(EventArgs e)
        {
            RaiseEvent(Started, this, e);
        }

        protected internal void RaiseEvent<T>(EventHandler<T> handler, object sender, T args)
            where T : EventArgs
        {
            if (handler != null)
                handler(sender, args);
        }

        #region ISerializableResource

        public void SerializeXml(IResourceProvider resourceProvider, XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        }

        public void DeserializeXml(IResourceProvider resourceProvider, XmlReader xmlReader)
        {
            Name = xmlReader.GetAttribute("Name");
            WrapMode wrapMode;
            if (Enum.TryParse(xmlReader.GetAttribute("WrapMode"), out wrapMode))
                WrapMode = wrapMode;
            else WrapMode = WrapMode.Once;
            
            xmlReader.ReadStartElement(); 

            while (xmlReader.IsStartElement())
            {
                string animationType = string.Format("Odyssey.Animations.{0}", xmlReader.LocalName);
                ISerializableResource curve;
                try
                {
                    curve = (ISerializableResource) Activator.CreateInstance(Type.GetType(animationType));
                }
                catch (ArgumentNullException)
                {
                    throw new InvalidOperationException(string.Format("Animation type `{0}` is not valid", animationType));
                }
                curve.DeserializeXml(resourceProvider, xmlReader);
                var animationCurve = (IAnimationCurve) curve;
                animationCurve.Name = string.Format("{0}Curve{1:D2}", Name, animationCurves.Count + 1);
                AddCurve(animationCurve);
            }

            xmlReader.ReadEndElement();
        }

        #endregion
    }
}