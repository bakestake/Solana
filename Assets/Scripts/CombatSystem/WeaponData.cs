using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite weaponSprite;
    public float attackRange = 1f;
    public float attackCooldown = 0.5f;
    public int damage = 10;
    public float attackSwingTime = 0.3f;
    public float attackAngle = 90f;

    // Shove settings
    public bool applyShove = false; // Enable/Disable shove
    public float shoveForce = 5f; // Intensity of shove
}
