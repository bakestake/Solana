using UnityEngine;

public abstract class BuffEffect : ScriptableObject
{
    // This method will be called when the buff is applied
    public abstract void Apply(GameObject target);

    // This method will be called when the buff expires
    public abstract void Remove(GameObject target);
}
