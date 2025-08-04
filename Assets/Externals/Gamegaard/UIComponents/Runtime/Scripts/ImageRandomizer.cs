using System.Collections.Generic;
using Gamegaard.Timer;
using Gamegaard.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace ALF.Piramipris
{
    public class ImageRandomizer : MonoBehaviour
    {
        [SerializeField] private Sprite[] images;
        [Min(0)]
        [SerializeField] private float imageDuration = 1;

        private Image image;
        private List<Sprite> usedImages = new List<Sprite>();
        private LoopTimer timer;
        private int RemainingAmount => images.Length - usedImages.Count;

        private void Awake()
        {
            timer = new LoopTimer(imageDuration);
            image = GetComponent<Image>();
        }

        private void Update()
        {
            if (timer.CheckAndUpdateTimer())
            {
                GetRandomImage();
            }
        }

        private void GetRandomImage()
        {
            //Sprite newImage = images.GetRandomExcept(usedImages);
            //if (RemainingAmount <= 1)
            //{
            //    usedImages.Clear();
            //}
            //usedImages.Add(newImage);
            //image.sprite = newImage;
        }
    }
}
