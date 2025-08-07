using UnityEngine;

[CreateAssetMenu(fileName = "NewDrunkEffect", menuName = "Buffs/Effects/Drunk Effect")]
public class DrunkEffect : BuffEffect
{
    public override void Apply(GameObject target)
    {
        LocalGameManager.Instance.EnableWeedHighEffect();
    }

    public override void Remove(GameObject target)
    {
        LocalGameManager.Instance.DisableWeedHighEffect();
    }
}
