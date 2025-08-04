using Febucci.UI;
using UnityEngine;

namespace Bakeland
{
    public class TypewriterWrapper : MonoBehaviour
    {
        [HideInInspector] public string text;

        [SerializeField] private TypewriterByCharacter typewriter;

        public void StartTypewriter()
        {
            typewriter.ShowText(text);
        }
    }
}