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
using System.Reflection;
using Odyssey.Content;
using Odyssey.Engine;
using Odyssey.Utilities.Reflection;

#endregion

namespace Odyssey.Animations
{
    public class AnimationController : IAnimatable
    {
        private string name;
        private readonly Dictionary<string, Animation> animations;

        private readonly List<Animation> playingAnimations;
        private readonly Dictionary<string, ObjectWalker> walkers;
        private object target;

        public AnimationController(object target) : this()
        {
            this.target = target;
        }

        public AnimationController()
        {
            animations = new Dictionary<string, Animation>();
            walkers = new Dictionary<string, ObjectWalker>();
            playingAnimations = new List<Animation>();
        }

        public string Name
        {
            get { return name; }
        }

        public IEnumerable<Animation> Animations
        {
            get { return animations.Values; }
        }

        public object Target
        {
            get { return target; }
            set
            {
                if (target == value)
                    return;
                target = value;
                foreach (var kvp in walkers)
                {
                    string targetPropertyName = kvp.Key.Split('.').Last();
                    var walker = kvp.Value;
                    walker.SetTarget(target, targetPropertyName);
                }
            }
        }

        public bool HasAnimations
        {
            get { return animations.Count > 0; }
        }

        public bool IsPlaying { get; private set; }

        public Animation this[string animationName]
        {
            get { return animations[animationName]; }
        }

        public Animation this[int index]
        {
            get { return animations.Values.ElementAt(index); }
        }

        [Pure]
        public bool ContainsAnimation(string animationName)
        {
            return animations.ContainsKey(animationName);
        }

        public void AddAnimation(Animation animation)
        {
            Contract.Requires<ArgumentNullException>(animation != null, "animation");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(animation.Name), "Cannot add an unnamed animation");
            Contract.Requires<ArgumentException>(!ContainsAnimation(animation.Name),
                "An animation with the same name alrady exists in the collection");
            animations.Add(animation.Name, animation);
            animation.Completed += (s, e) => Stop(animation);
        }

        public void AddAnimations(IEnumerable<Animation> animations)
        {
            Contract.Requires<ArgumentNullException>(animations != null, "animations");
            foreach (var animation in animations)
                AddAnimation(animation);
        }

        public virtual void Initialize()
        {
            foreach (var animation in animations.Values)
            {
                int cachedAnimations = 0;
                List<IAnimationCurve> curves = new List<IAnimationCurve>(animation.Curves);
                foreach (IAnimationCurve curve in curves)
                {
                    object realTarget;

                    if (string.IsNullOrEmpty(curve.TargetName))
                        realTarget = target;
                    else
                    {
                        var resourceProvider = target as IResourceProvider;
                        if (resourceProvider != null)
                            realTarget = resourceProvider.GetResource<IResource>(curve.TargetName);
                        else
                            throw new InvalidOperationException(string.Format("'Target' does not implement {0}", typeof (IResourceProvider)));
                    }

                    if (realTarget == null)
                        throw new InvalidOperationException("'Target' cannot be null");

                    name = string.Format("{0}.Animator", ((IResource) realTarget).Name);

                    ObjectWalker walker = new ObjectWalker(realTarget, curve.TargetProperty);

                    var requiresCaching = realTarget as IRequiresCaching;
                    string curveKey = curve.Key;
                    if (requiresCaching != null)
                    {
                        var newCurve = requiresCaching.CacheAnimation(walker.CurrentMember.DeclaringType, walker.CurrentMember.Name, curve);
                        if (newCurve != null)
                        {
                            animation.RemoveCurve(curve.TargetProperty);
                            animation.AddCurve(newCurve);
                            walker.SetTarget(requiresCaching, newCurve.TargetProperty);
                            curveKey = newCurve.Key;
                            cachedAnimations++;
                        }
                    }

                    var animatable = walker.CurrentMember.GetCustomAttribute<AnimatableAttribute>();
                    if (animatable == null)
                        throw new InvalidOperationException(string.Format("'{0}' is not marked as {1}", walker.CurrentMember.Name,
                            typeof (AnimatableAttribute).Name));

                    walkers.Add(curveKey, walker);
                }

                if (cachedAnimations > 0 && cachedAnimations != curves.Count)
                    throw new InvalidOperationException(string.Format("Mixing cached and uncached animations is not supported"));
            }
        }

        public virtual void Play()
        {
            Reset();
            playingAnimations.AddRange(animations.Values);
            foreach (var animation in playingAnimations)
                animation.Start();
            IsPlaying = true;
        }

        private void Play(Animation animation)
        {
            animation.Rewind();
            if (!playingAnimations.Contains(animation))
                playingAnimations.Add(animation);
            animation.Start();
        }

        public void Play(string animationName)
        {
            Contract.Requires<ArgumentNullException>(ContainsAnimation(animationName), "Animation not found");
            var animation = animations[animationName];
            Play(animation);
            IsPlaying = true;
        }

        private void Reset()
        {
            IsPlaying = false;
            playingAnimations.Clear();
        }

        public void Stop()
        {
            foreach (var animation in playingAnimations)
            {
                animation.Stop();
            }
            Reset();

        }

        private void Stop(Animation animation)
        {
            playingAnimations.Remove(animation);
            if (playingAnimations.Count == 0)
            {
                Reset();
            }
        }

        public void Stop(string animationName)
        {
            Contract.Requires<ArgumentNullException>(ContainsAnimation(animationName), "Animation not found");
            var animation = animations[animationName];
            Stop(animation);
        }

        public void Rewind(string animationName)
        {
            Contract.Requires<ArgumentNullException>(ContainsAnimation(animationName), "Animation not found");
            animations[animationName].Rewind();
        }

        public virtual void Update(ITimeService time)
        {
            var currentlyPlayingAnimations = new List<Animation>(playingAnimations);

            foreach (var animation in currentlyPlayingAnimations)
            {
                animation.Update(time, (curve, value) =>
                {
                    var walker = GetWalker(curve.Key);
                    walker.WriteValue(value);
                });
            }
        }

        private ObjectWalker GetWalker(string name)
        {
            return walkers[name];
        }
    }
}