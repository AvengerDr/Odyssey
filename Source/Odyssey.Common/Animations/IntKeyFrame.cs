using System.Globalization;
using Odyssey.Serialization;

namespace Odyssey.Animations
{
    public class IntKeyFrame : KeyFrame<int>
    {
        protected override void OnReadXml(XmlDeserializationEventArgs e)
        {
            base.OnReadXml(e);
            string value = e.XmlReader.GetAttribute("Value");
            Value = int.Parse(value, CultureInfo.InvariantCulture);

            e.XmlReader.ReadStartElement();
        }
    }
}
