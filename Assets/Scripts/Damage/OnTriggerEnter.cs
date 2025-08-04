using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Bakeland
{
    public class OnTriggerEnter : MonoBehaviour
    {
        [SerializeField] private string[] validTags;
        [SerializeField] private UnityEvent<Collider2D> OnObjectTriggerEnter;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(validTags.Length == 0 || validTags.Contains(collision.tag))
            {
                OnObjectTriggerEnter?.Invoke(collision);
            }
        }
    }
}
