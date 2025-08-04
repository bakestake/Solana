using UnityEngine;

public interface IColorable
{
    public Color CurrentColor { get; }
    public void SetColor(Color color);
}
