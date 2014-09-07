using System;
using Odyssey.Engine;

namespace Odyssey.Animations
{
    public interface IAnimationCurve {
        string TargetProperty { get; }
        string TargetName { get; }
        string Name { get; set; }

        float Duration { get; }
        object Evaluate(float elapsedTime);
        IKeyFrame this[int index] { get; }
    }
}