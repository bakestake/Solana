using UnityEngine;

namespace Gamegaard.CursorSystem
{
    public class VirtualCursorManager : GenericCursorManagerBase<SpriteCursorData, Sprite>
    {
        [SerializeField] private VirtualCursor currentVirtualCursor;
        [SerializeField] private CursorCanvas cursorCanvas;

        public CursorCanvas CursorCanvas => cursorCanvas;
        public VirtualCursor VirtualCursor => currentVirtualCursor;

        protected override void Reset()
        {
            cursorCanvas = GetComponentInChildren<CursorCanvas>();
            currentVirtualCursor = GetComponentInChildren<VirtualCursor>();
        }

        public void SetVirtualCursor(VirtualCursor cursor)
        {
            currentVirtualCursor = cursor;
        }

        public void SetCursorCanvas(CursorCanvas cursorCanvas)
        {
            this.cursorCanvas = cursorCanvas;
        }

        public void SetColor(Color color)
        {
            currentVirtualCursor.SetColor(color);
        }

        public override void SetCursorToScreenPosition(Vector2 position, int cursorID = 0)
        {
            currentVirtualCursor.SetPosition(position);
        }

        public override void ShowCursor(int cursorID = 0)
        {
            currentVirtualCursor.Show();
        }

        public override void HideCursor(int cursorID = 0)
        {
            currentVirtualCursor.Hide();
        }

        protected override void ApplyState(CursorState<Sprite> state)
        {
            if (currentVirtualCursor != null)
            {
                currentVirtualCursor.SetSprite(state.Frames[0]);
                currentVirtualCursor.RectTransform.pivot = new Vector2(state.Hotspot.x, 1 - state.Hotspot.y);
                currentVirtualCursor.enabled = true;
            }
        }

        protected override void ApplyFrame(CursorState<Sprite> state, int frameIndex)
        {
            if (currentVirtualCursor != null)
            {
                currentVirtualCursor.SetSprite(state.Frames[frameIndex]);
            }
        }
    }
}