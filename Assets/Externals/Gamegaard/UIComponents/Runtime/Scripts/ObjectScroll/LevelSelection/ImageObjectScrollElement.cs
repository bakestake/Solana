using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard
{
    public class ImageObjectScrollElement : ObjectScrollElement<Sprite>
    {
        [SerializeField] private Image packImage;

        public override void Initialize(Sprite value)
        {
            packImage.sprite = value;
        }
    }
}