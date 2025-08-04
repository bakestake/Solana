using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Cmp;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bakeland
{
    public class ItemPickupPopupElement : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;

        public void Initialize(Sprite itemSprite, string itemName)
        {
            image.sprite = itemSprite;
            text.text = $"{itemName} added";
            StartCoroutine(Coroutine_Pop(3f, 1.5f, 5f));
        }

        private IEnumerator Coroutine_Pop(float speed, float popAmount, float durationBeforeDestroy)
        {
            this.transform.localScale = Vector3.zero;

            // increase scale until max amount
            while (this.transform.localScale.x < popAmount)
            {
                this.transform.localScale += Vector3.one * (Time.deltaTime * speed);
                yield return null;
            }

            // return scale to 1
            while (this.transform.localScale.x > 1f)
            {
                this.transform.localScale -= Vector3.one * (Time.deltaTime * speed);
                yield return null;
            }
            this.transform.localScale = Vector3.one;

            // wait before scaling down to 0 and then being destroyed
            yield return new WaitForSeconds(durationBeforeDestroy);

            // scale down to 0 before destroying
            while (this.transform.localScale.x > 0f)
            {
                this.transform.localScale -= Vector3.one * (Time.deltaTime * speed);
                yield return null;
            }
            Destroy(this.gameObject);
        }
    }
}