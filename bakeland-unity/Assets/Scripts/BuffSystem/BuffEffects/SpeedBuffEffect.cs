using UnityEngine;

[CreateAssetMenu(fileName = "NewSpeedBuffEffect", menuName = "Buffs/Effects/Speed Buff")]
public class SpeedBuffEffect : BuffEffect
{
    public float speedMultiplier = 1.5f;  // The speed increase multiplier

    public override void Apply(GameObject target)
    {
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.moveSpeedMultiplier = speedMultiplier;
            player.transform.Find("SpeedBubbles").GetComponent<ParticleSystem>().Play();
        }
    }

    public override void Remove(GameObject target)
    {
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.moveSpeedMultiplier = 1;
            player.transform.Find("SpeedBubbles").GetComponent<ParticleSystem>().Stop();
        }
    }
}
