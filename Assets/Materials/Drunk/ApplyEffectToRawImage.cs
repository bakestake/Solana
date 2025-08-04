using UnityEngine;
using UnityEngine.UI;

public class ApplyEffectToRawImage : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;         // Reference to the RawImage UI element
    [SerializeField] private Material drunkEffectMaterial; // Material with the "DrunkEffect" shader
    [SerializeField] private Texture inputTexture;        // The texture you're applying the effect to (RenderTexture or Texture2D)

    void Start()
    {
        // Assign the input texture to the RawImage
        rawImage.texture = inputTexture;

        // Assign the drunk effect material to the RawImage's material
        rawImage.material = drunkEffectMaterial;
    }

    // Optionally, change effect parameters during gameplay
    public void SetEffectParameters(float swirlStrength, float swirlSpeed, float chromaticAberration)
    {
        drunkEffectMaterial.SetFloat("_SwirlStrength", swirlStrength);
        drunkEffectMaterial.SetFloat("_SwirlSpeed", swirlSpeed);
        drunkEffectMaterial.SetFloat("_ChromaticAberration", chromaticAberration);
    }
}
