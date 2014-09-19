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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml;
using Odyssey.Content;
using Odyssey.Geometry;
using Odyssey.Serialization;
using Odyssey.Utilities.Reflection;
using Odyssey.Utilities.Text;

#endregion

namespace Odyssey.Animations
{
    public abstract class AnimationCurve<TKeyFrame> : ISerializableResource, IResource, IAnimationCurve, IEnumerable<TKeyFrame>
        where TKeyFrame : class, IKeyFrame
    {
        public delegate object CurveFunction(TKeyFrame start, TKeyFrame end, float time, object options = null);

        private string name;
        private readonly List<TKeyFrame> keyFrames;
        private object functionOptions;

        protected object FunctionOptions
        {
            get { return functionOptions; }
        }

        public AnimationCurve()
        {
            keyFrames = new List<TKeyFrame>();
        }

        public string Key { get { return string.Format("{0}.{1}", Name, TargetProperty); } }

        public int KeyFrameCount
        {
            get { return keyFrames.Count; }
        }

        public CurveFunction Function { get; set; }

        /// <inheritdoc/>
        public float Duration
        {
            get { return keyFrames.Max(kf => kf.Time); }
        }

        public string TargetProperty { get; set; }
        public string TargetName { get; internal set; }

        public virtual object Evaluate(float time)
        {
            TKeyFrame start = keyFrames.First();
            TKeyFrame end = keyFrames.Last();
            if (time <= start.Time)
                return start.Value;
            else if (time > end.Time)
                return end.Value;

            start = keyFrames.LastOrDefault(kf => kf.Time < time) ?? start;
            end = keyFrames.First(kf => kf.Time > start.Time);
            object result = Function(start, end, time, functionOptions);

            return result;
        }

        public IKeyFrame this[int index]
        {
            get { return keyFrames[index]; }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (string.Equals(name, value))
                    return;
                name = value;
            }
        }

        #region IEnumerable<TKeyFrame>

        public IEnumerator<TKeyFrame> GetEnumerator()
        {
            return ((IEnumerable<TKeyFrame>) keyFrames).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return keyFrames.GetEnumerator();
        }

        #endregion

        #region IResourceProvider

        public void SerializeXml(IResourceProvider resourceProvider, XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        }

        protected virtual object DeserializeOptions(string methodName, string options, IResourceProvider resourceProvider)
        {
            throw new InvalidOperationException(string.Format("Method '{0}' not supported", methodName));
        }

        public void DeserializeXml(IResourceProvider resourceProvider, XmlReader xmlReader)
        {
            TargetProperty = xmlReader.GetAttribute("TargetProperty");

            TargetName = Text.ParseResource(xmlReader.GetAttribute("TargetName"));
            if (!resourceProvider.ContainsResource(TargetName))
                throw new InvalidOperationException(String.Format("No resource '{0}' found", TargetName));

            string function = xmlReader.GetAttribute("Function");
            if (!string.IsNullOrEmpty(function))
            {
                // It seems that Type.GetMethods does not return inherited methods
                var method = ReflectionHelper.GetMethod(GetType(), function) ??
                             ReflectionHelper.GetMethod(typeof (AnimationCurve<TKeyFrame>), function);

                Function = (CurveFunction) method.CreateDelegate(typeof (CurveFunction));
                string options = xmlReader.GetAttribute("Options");
                functionOptions = DeserializeOptions(method.Name, options, resourceProvider);
            }

            xmlReader.ReadStartElement();

            while (xmlReader.IsStartElement())
            {
                string type = xmlReader.LocalName;
                var keyFrame = (ISerializableResource)
                        Activator.CreateInstance(Type.GetType(String.Format("Odyssey.Animations.{0}, Odyssey.Common", type)));
                keyFrame.DeserializeXml(resourceProvider, xmlReader);
                AddKeyFrame((TKeyFrame) keyFrame);
            }

            xmlReader.ReadEndElement();
        }

        #endregion IXmlSerializable

        public void AddKeyFrame(TKeyFrame keyFrame)
        {
            Contract.Requires<ArgumentNullException>(keyFrame != null, "keyFrame");
            keyFrames.Add(keyFrame);
            keyFrames.Sort();
        }

        public void Clear()
        {
            keyFrames.Clear();
        }

        protected static float Map(float start, float end, float time)
        {
            float denominator = end - start;
            if (MathHelper.ScalarNearEqual(denominator, 0))
                return 0;

            float result = (time - start)/denominator;
            return MathHelper.Clamp(result, 0, 1);
        }

        public static object Discrete(TKeyFrame start, TKeyFrame end, float time, object options = null)
        {
            return time < end.Time ? start.Value : end.Value;
        }

        public static object SquareWave(TKeyFrame start, TKeyFrame end, float time, object options = null)
        {
            Contract.Requires<ArgumentNullException>(options != null, "options");
            float subdivisions = (float) options;
            float period = (end.Time - start.Time)/subdivisions;
            return (time%period) < (period/2) ? start.Value : end.Value;
        }
    }
}