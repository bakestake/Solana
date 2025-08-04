using UnityEngine;

namespace Gamegaard.DynamicValues
{
    [System.Serializable]
    public class ModifierApplierBase
    {
#if UNITY_EDITOR
        [SerializeField, HideInInspector] private string EditorName;
#endif
        public virtual string GetDescription() 
        {
            return "";
        }
    }
}