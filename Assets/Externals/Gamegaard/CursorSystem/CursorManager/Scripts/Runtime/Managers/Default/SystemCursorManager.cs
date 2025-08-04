using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Gamegaard.CursorSystem
{
    public class SystemCursorManager : GenericCursorManagerBase<TextureCursorData, Texture2D>
    {
        [SerializeField] private CursorMode cursorMode;

        public override void SetCursorToScreenPosition(Vector2 position, int cursorID = 0)
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                Mouse.current.WarpCursorPosition(position);
            }
            else
            {
                Debug.LogWarning("No mouse detected. WarpCursorPosition cannot be used.");
            }
#else
            Debug.LogWarning("New Input System is not enabled. WarpCursorPosition is unavailable.");
#endif
        }

        public override void ShowCursor(int cursorID = 0)
        {
            Cursor.visible = true;
        }

        public override void HideCursor(int cursorID = 0)
        {
            Cursor.visible = false;
        }

        protected override void ApplyState(CursorState<Texture2D> state)
        {
            SetCursor(state, 0);
        }

        protected override void ApplyFrame(CursorState<Texture2D> state, int frameIndex)
        {
            if (frameIndex < state.FrameCount)
            {
                SetCursor(state, frameIndex);
            }
        }

        private void SetCursor(CursorState<Texture2D> state, int frameIndex)
        {
            Texture2D texture = state.Frames[frameIndex];
            Vector2 center = state.Hotspot * new Vector2(texture.width, texture.height);
            Cursor.SetCursor(texture, center, cursorMode);
        }
    }
}