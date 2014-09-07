using Odyssey.Serialization;

namespace Odyssey.Animations
{
    public class BoolKeyFrame : KeyFrame<bool>
    {
        protected override void OnReadXml(XmlDeserializationEventArgs e)
        {
            base.OnReadXml(e);
            string value = e.XmlReader.GetAttribute("Value");
            Value = bool.Parse(value);

            e.XmlReader.ReadStartElement();
        }
    }
}
