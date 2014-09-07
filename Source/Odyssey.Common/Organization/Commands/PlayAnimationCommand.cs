using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using Odyssey.Animations;
using Odyssey.Content;
using Odyssey.Graphics.Organization;
using Odyssey.Serialization;
using SharpDX;

namespace Odyssey.Organization.Commands
{
    public class PlayAnimationCommand : Command, ISerializableResource
    {
        private IAnimatable target;
        private string animationName;
        private float time;

        public IAnimatable Target
        {
            get { return target; }
        }

        public string AnimationName
        {
            get { return animationName; }
        }

        public float Time
        {
            get { return time; }
        }

        public PlayAnimationCommand(IServiceRegistry services)
            : base(services, CommandType.PlayAnimation)
        {
        }

        public PlayAnimationCommand(IServiceRegistry services, AnimationController target, string animationName, float startTime)
            : this(services)
        {
            Contract.Requires<ArgumentNullException>(target != null, "target");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(animationName), "animationName");
            this.target = target;
            this.animationName = animationName;
            time = startTime;
        }

        public override void Initialize()
        {
        }

        public override void Execute()
        {
            if (string.IsNullOrEmpty(animationName))
                target.Play();
            else
                target.Play(animationName);
        }

        public void SerializeXml(IResourceProvider resourceProvider, System.Xml.XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        }

        public void DeserializeXml(IResourceProvider resourceProvider, System.Xml.XmlReader xmlReader)
        {
            string targetName = xmlReader.GetAttribute("TargetName");
            target = resourceProvider.GetResource<IAnimatable>(targetName);
            animationName = xmlReader.GetAttribute("AnimationName");
            time = float.Parse(xmlReader.GetAttribute("Time"), CultureInfo.InvariantCulture);
            xmlReader.ReadStartElement();
        }
    }
}
