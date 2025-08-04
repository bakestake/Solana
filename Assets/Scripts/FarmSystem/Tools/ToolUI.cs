using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard
{
    public class ToolUI : MonoBehaviour
    {
        [SerializeField] private Image toolImage;
        public void SetTool(Tool tool)
        {
            toolImage.sprite = tool.ToolSprite;
        }
    }
}