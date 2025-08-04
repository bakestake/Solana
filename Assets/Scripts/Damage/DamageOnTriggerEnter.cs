using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Bakeland
{
    public class DamageOnTriggerEnter : MonoBehaviour
    {
        [SerializeField] private int damage;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Health health))
            {
                health.TakeDamage(damage, transform.position);
            }
        }
    }
}
