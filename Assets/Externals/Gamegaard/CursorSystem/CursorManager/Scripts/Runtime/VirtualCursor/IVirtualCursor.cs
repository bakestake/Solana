using UnityEngine;

namespace Gamegaard.CursorSystem
{
    public interface IVirtualCursor
    {
        void Show();
        void Hide();
        void SetPosition(Vector3 position);
        void SetSprite(Sprite sprite);
        void SetColor(Color color);
    }
}
