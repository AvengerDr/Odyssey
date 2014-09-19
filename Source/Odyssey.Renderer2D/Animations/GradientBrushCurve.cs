using System;
using System.Text.RegularExpressions;
using Odyssey.Engine;
using Odyssey.Graphics;
using Odyssey.UserInterface.Style;

namespace Odyssey.Animations
{
    public class GradientBrushCurve : AnimationCurve<GradientBrushKeyFrame>
    {
        public GradientBrushCurve()
        {
            Function = Discrete;
        }

        public static object Discrete(GradientBrushKeyFrame start, GradientBrushKeyFrame end, float time)
        {
            return end.Value;
        }

        internal static IAnimationCurve FromColor4Curve<TGradientBrush>(Direct2DDevice device, Color4Curve originalCurve,
            TGradientBrush originalBrush)
            where TGradientBrush : GradientBrush
        {
            var styleService = device.Services.GetService<IStyleService>();

            var match = Regex.Match(originalCurve.TargetProperty, @"\w*\[(?<index>\d+)\]");
            int index = int.Parse(match.Groups["index"].Value);
            var newCurve = new GradientBrushCurve() { Name = originalCurve.Name };
            int keyFrameIndex = 0;
            foreach (var keyframe in originalCurve)
            {
                var newGradient = (Gradient)originalBrush.Gradient.CopyAs(string.Format("{0}.Keyframe{1:D2}", newCurve.Name, keyFrameIndex++));
                newGradient.GradientStops[index].Color = keyframe.Value;
                var newBrush = Brush.FromColorResource(device, newGradient);
                newBrush.Initialize();
                styleService.AddResource(newBrush);

                var newKeyFrame = new GradientBrushKeyFrame() { Time = keyframe.Time, Value = (TGradientBrush)newBrush };
                newCurve.AddKeyFrame(newKeyFrame);
            }
            newCurve.TargetName = originalCurve.TargetName;
            newCurve.TargetProperty = originalCurve.TargetProperty.Split('.')[0];
            return newCurve;
        }
    }
}
