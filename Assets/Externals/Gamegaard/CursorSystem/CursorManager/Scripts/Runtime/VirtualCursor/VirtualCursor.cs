using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard.CursorSystem
{
    public class VirtualCursor : MonoBehaviour, IVirtualCursor
    {
        [Header("Settings")]
        [SerializeField] private bool check2D = true;
        [SerializeField] private bool check3D = true;
        [SerializeField] private LayerMask interactableLayers;
        [SerializeField] private float raycastDistance = 100;

        [Header("References")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Camera targetCamera;
        [SerializeField] private Canvas cursorCanvas;
        [SerializeField] private Image virtualCursorImage;

        private GameObject lastHoveredObject3D;
        private GameObject lastHoveredObject2D;

        public CursorIdentifierData ID { get; private set; } = CursorIdentifierData.playerOne;
        public RectTransform RectTransform => rectTransform;

        private void Reset()
        {
            targetCamera = Camera.main;
            cursorCanvas = GetComponentInParent<Canvas>();
            virtualCursorImage = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (targetCamera == null) return;

            if (check3D)
            {
                Check3D();
            }

            if (check2D)
            {
                Check2D();
            }
        }

        public void SetID(CursorIdentifierData Id)
        {
            name = $"Cursor [{Id.CursorID}]";
            ID = Id;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetSprite(Sprite sprite)
        {
            virtualCursorImage.sprite = sprite;
        }

        public void SetColor(Color color)
        {
            virtualCursorImage.color = color;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        private void Check3D()
        {
            Ray ray = targetCamera.ScreenPointToRay(transform.position);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, interactableLayers))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (lastHoveredObject3D != hitObject)
                {
                    if (lastHoveredObject3D != null)
                    {
                        TriggerMouseExit(lastHoveredObject3D);
                    }

                    TriggerMouseEnter(hitObject);
                    lastHoveredObject3D = hitObject;
                }
            }
            else
            {
                if (lastHoveredObject3D != null)
                {
                    TriggerMouseExit(lastHoveredObject3D);
                    lastHoveredObject3D = null;
                }
            }
        }

        private void Check2D()
        {
            Vector2 worldPoint;

            if (cursorCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                worldPoint = targetCamera.ScreenToWorldPoint(transform.position + new Vector3(0, 0, -targetCamera.transform.position.z));
            }
            else
            {
                worldPoint = transform.position;
            }

            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, raycastDistance, interactableLayers);

            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;

                if (lastHoveredObject2D != hitObject)
                {
                    if (lastHoveredObject2D != null)
                    {
                        TriggerMouseExit(lastHoveredObject2D);
                    }

                    TriggerMouseEnter(hitObject);
                    lastHoveredObject2D = hitObject;
                }
            }
            else
            {
                if (lastHoveredObject2D != null)
                {
                    TriggerMouseExit(lastHoveredObject2D);
                    lastHoveredObject2D = null;
                }
            }
        }

        private void TriggerMouseEnter(GameObject obj)
        {
            if (ID.CursorID == 0)
            {
                obj.SendMessage("OnMouseEnter", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                obj.SendMessage("OnMultiplayerMouseEnter", ID, SendMessageOptions.DontRequireReceiver);
            }
        }

        private void TriggerMouseExit(GameObject obj)
        {
            if (ID.CursorID == 0)
            {
                obj.SendMessage("OnMouseExit", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                obj.SendMessage("OnMultiplayerMouseExit", ID, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}