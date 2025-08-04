using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HitboxVisual : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Material material;
    [SerializeField] private Color hitboxColor = new Color(1f, 1f, 1f, 0.5f);
    private const int TEXTURE_SIZE = 128;
    private Vector2 attackDirection = Vector2.right;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Load shader and create material
        Shader hitboxShader = Shader.Find("Custom/HitboxWave");
        if (hitboxShader == null)
        {
            Debug.LogError("Could not find shader 'Custom/HitboxWave'. Using fallback shader.");
            material = new Material(Shader.Find("Sprites/Default"));
        }
        else
        {
            material = new Material(hitboxShader);
        }

        // Create and assign sprite
        Sprite defaultSprite = CreateCircleSprite();
        spriteRenderer.sprite = defaultSprite;
        spriteRenderer.material = material;
        material.SetColor("_Color", hitboxColor);

        // Ensure it renders on top of other sprites
        spriteRenderer.sortingOrder = 100;
    }

    private Sprite CreateCircleSprite()
    {
        Texture2D texture = new Texture2D(TEXTURE_SIZE, TEXTURE_SIZE);

        float centerX = TEXTURE_SIZE / 2f;
        float centerY = TEXTURE_SIZE / 2f;
        float radius = TEXTURE_SIZE / 2f;

        // Create circle texture
        for (int x = 0; x < TEXTURE_SIZE; x++)
        {
            for (int y = 0; y < TEXTURE_SIZE; y++)
            {
                float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                if (distance < radius)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        texture.Apply();

        // Create sprite with pixels per unit set to match texture size
        return Sprite.Create(
            texture,
            new Rect(0, 0, TEXTURE_SIZE, TEXTURE_SIZE),
            new Vector2(0.5f, 0.5f),
            TEXTURE_SIZE
        );
    }

    public void GenerateFromCollider(Collider2D collider)
    {
        if (collider is BoxCollider2D boxCollider)
        {
            transform.localPosition = boxCollider.offset;
            transform.localScale = boxCollider.size;
            Debug.Log($"Created box visual with size: {boxCollider.size}");
        }
        else if (collider is CircleCollider2D circleCollider)
        {
            transform.localPosition = circleCollider.offset;
            transform.localScale = Vector3.one * (circleCollider.radius * 2);
            Debug.Log($"Created circle visual with radius: {circleCollider.radius}");
        }
    }

    public void StartWaveEffect(Vector2 direction)
    {
        if (material.HasProperty("_WaveStart"))
        {
            material.SetFloat("_WaveStart", Time.time);
            material.SetVector("_Direction", direction);
        }
    }
}