namespace Odyssey.Graphics
{
    public interface IColorResource {
        ColorType Type { get; }
        float Opacity { get; set; }
    }
}