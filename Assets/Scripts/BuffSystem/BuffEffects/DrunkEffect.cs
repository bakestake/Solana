using Bakeland;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDrunkEffect", menuName = "Buffs/Effects/Drunk Effect")]
public class DrunkEffect : BuffEffect
{
    public override void Apply(GameObject target)
    {
        PostProcessingManager.Instance.ShowIntoxicated();
    }

    public override void Remove(GameObject target)
    {
        PostProcessingManager.Instance.HideIntoxicated();
    }
}
