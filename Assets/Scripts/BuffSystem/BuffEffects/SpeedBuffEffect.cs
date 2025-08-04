using UnityEngine;

[CreateAssetMenu(fileName = "NewSpeedBuffEffect", menuName = "Buffs/Effects/Speed Buff")]
public class SpeedBuffEffect : BuffEffect
{
    public float speedMultiplier = 1.5f;

    public override void Apply(GameObject target)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.MoveSpeedMultiplier = speedMultiplier;
            player.transform.Find("SpeedBubbles").GetComponent<ParticleSystem>().Play();
        }
    }

    public override void Remove(GameObject target)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.MoveSpeedMultiplier = 1;
            player.transform.Find("SpeedBubbles").GetComponent<ParticleSystem>().Stop();
        }
    }
}
