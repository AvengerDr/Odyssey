using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Odyssey.Geometry;
using Odyssey.Serialization;
using SharpDX;

namespace Odyssey.Animations
{
    public class QuaternionKeyFrame : KeyFrame<Quaternion>
    {
        protected override void OnReadXml(XmlDeserializationEventArgs e)
        {
            base.OnReadXml(e);
            string value = e.XmlReader.GetAttribute("Value");

            const string pattern = @"(?<notation>[QE]+)\s*(?<values>[0-9.,\s]*)";
            Regex regex = new Regex(pattern);
            var matches = regex.Match(value);
            char notation = matches.Groups["notation"].Value[0];
            string[] values = matches.Groups["values"].Value.Split(',');

            Quaternion result;
            float[] qValues = (from s in values select float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
            if (notation == 'E')
            {
                if (values.Length != 3)
                    throw new InvalidOperationException(string.Format("Euler values are not in the correct format: {0}", value));

                result = Quaternion.RotationYawPitchRoll(MathHelper.DegreesToRadians(qValues[0]), MathHelper.DegreesToRadians(qValues[1]),
                    MathHelper.DegreesToRadians(qValues[2]));
            }
            else if (notation=='Q')
            {
                if (values.Length != 4)
                    throw new InvalidOperationException(string.Format("Quaternion values are not in the correct format: {0}", value));
                result = new Quaternion(qValues);
            }
            else throw new InvalidOperationException(string.Format("Notation '{0}' not recognized",notation));
            e.XmlReader.ReadStartElement();
            Value = result;
        }
    }
}
