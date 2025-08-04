using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace Gamegaard.CursorSystem
{
    public class VirtualCursorArea : MonoBehaviour
    {
        [SerializeField] private RectTransform canvasRectTransform;
        [SerializeField] private RectTransform cursorRectTransform;
        [SerializeField] private VirtualMouseFix virtualMouseInput;

        private float lastScale;

        private void Update()
        {
            float currentScale = canvasRectTransform.localScale.x;

            if (currentScale != lastScale)
            {
                cursorRectTransform.localScale = Vector3.one * currentScale;
                transform.localScale = Vector3.one * (1f / currentScale);
                lastScale = currentScale;
            }
        }

        private void LateUpdate()
        {
            Vector2 virtualMousePosition = virtualMouseInput.VirtualMouse.position.value;
            virtualMousePosition.x = Mathf.Clamp(virtualMousePosition.x, 0, Screen.width);
            virtualMousePosition.y = Mathf.Clamp(virtualMousePosition.y, 0, Screen.height);
            InputState.Change(virtualMouseInput.VirtualMouse.position, virtualMousePosition);
        }
    }
}