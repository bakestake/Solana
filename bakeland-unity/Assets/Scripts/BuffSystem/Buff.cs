using UnityEngine;

[CreateAssetMenu(fileName = "NewBuff", menuName = "Buffs/Buff")]
public class Buff : ScriptableObject
{
    public string buffName;               // Name of the buff
    public string description;            // Description of the buff
    public Sprite icon;                   // Icon to display in the UI
    public float defaultDuration;         // Default time for the buff
    public bool isDebuff;                 // Whether it's a debuff or a buff
    public BuffEffect buffEffect;         // The actual effect applied to the player

    // Method to apply the buff's effect
    public void ApplyEffect(GameObject target)
    {
        if (buffEffect != null)
        {
            buffEffect.Apply(target);
        }
    }

    // Method to remove the buff's effect
    public void RemoveEffect(GameObject target)
    {
        if (buffEffect != null)
        {
            buffEffect.Remove(target);
        }
    }
}
