using System;
using UnityEngine;

namespace Bakeland
{
    [ExecuteInEditMode]
    public class InstanceID : MonoBehaviour
    {
        [SerializeField] private string uniqueID = Guid.NewGuid().ToString();

        public string UniqueID { get => uniqueID; }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(uniqueID))
            {
                uniqueID = Guid.NewGuid().ToString();
            }
        }
    }
}