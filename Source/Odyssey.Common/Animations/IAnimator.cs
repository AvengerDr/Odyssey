using Odyssey.Content;

namespace Odyssey.Animations
{
    public interface IAnimator : IResource
    {
        AnimationController Animator { get; }
    }
}
