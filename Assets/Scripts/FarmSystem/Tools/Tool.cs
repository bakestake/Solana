using UnityEngine;

namespace Gamegaard
{
    [CreateAssetMenu(fileName = "Tool_", menuName = "Tools/Tool")]
    public class Tool : ScriptableObject
    {
        [SerializeField] private string toolName;
        [SerializeField] private string toolDescription;
        [SerializeField] private Sprite toolSprite;
        [SerializeField] private ToolBehaviour toolBehaviour;

        public string ToolName => toolName;
        public string ToolDescription => toolDescription;
        public Sprite ToolSprite => toolSprite;
        public ToolBehaviour ToolBehaviour => toolBehaviour;
    }
}