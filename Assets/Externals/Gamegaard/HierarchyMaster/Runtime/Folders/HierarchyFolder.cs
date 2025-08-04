using Gamegaard.HierarchyMaster.Attributes;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Gamegaard.HierarchyMaster.Editor")]
namespace Gamegaard.HierarchyMaster
{
    [HierarchyIcon]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    internal sealed class HierarchyFolder : EditorOnlyBehaviour
    {
        [SerializeField] private bool useCustomIcons;
        [SerializeField] private Texture2D customOpenFolderIcon;
        [SerializeField] private Texture2D customClosedFolderIcon;

        public bool IsReparentingEnabled { get; set; } = true;
        public bool UseCustomIcons => useCustomIcons;
        public Texture2D CustomOpenFolderIcon => customOpenFolderIcon;
        public Texture2D CustomClosedFolderIcon => customClosedFolderIcon;

        private void Awake()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        private void OnTransformParentChanged()
        {
            if (!IsReparentingEnabled) return;
            RecenterKeepingChildrenWorldPosition();
        }

        public void RecenterKeepingChildrenWorldPosition()
        {
            Transform[] children = new Transform[transform.childCount];
            Vector3[] childrenPositions = new Vector3[transform.childCount];
            Quaternion[] childrenRotations = new Quaternion[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                children[i] = transform.GetChild(i);
                childrenPositions[i] = children[i].position;
                childrenRotations[i] = children[i].rotation;
            }

            Vector3 originalPosition = transform.position;
            Quaternion originalRotation = transform.rotation;

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            transform.position = originalPosition;
            transform.rotation = originalRotation;

            for (int i = 0; i < children.Length; i++)
            {
                children[i].position = childrenPositions[i];
                children[i].rotation = childrenRotations[i];
            }

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void Strip(StrippingMode strippingMode)
        {
            switch (strippingMode)
            {
                case StrippingMode.Delete:
                    HierarchyFolderUtils.RemoveFolder(this);
                    break;
                case StrippingMode.ReplaceWithSeparator:
                    string name = $"------{gameObject.name.ToUpper()}------";
                    Transform newSeparator = new GameObject(name).transform;
                    newSeparator.parent = HierarchyFolderUtils.GetValidParent(transform);
                    newSeparator.SetSiblingIndex(transform.GetSiblingIndex());
                    HierarchyFolderUtils.RemoveFolder(this);
                    break;
            }
        }
    }
}