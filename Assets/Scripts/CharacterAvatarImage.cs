using UnityEngine;
using UnityEngine.UI;

namespace Bakeland
{
    public class CharacterAvatarImage : MonoBehaviour
    {
        private Image image;
        private RectTransform rectTransform;

        private void Awake()
        {
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            CharacterSelector.OnCharacterSelected += OnSelected;
        }

        public void OnSelected(CharacterSelectorChar character)
        {
            image.sprite = character.perfil;
            image.color = Color.white;

            rectTransform.sizeDelta = new Vector2(128, 128);
        }
    }
}
