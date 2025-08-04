using UnityEngine;

public class DizzyTriggerCaster : MonoBehaviour
{
    [SerializeField] private Buff drunkBuff;

    public void Trigger()
    {
        SoundManager.Instance.PlaySfx(SoundManager.Instance.weedUsed);
        BuffManager.instance.AddBuff(drunkBuff);
    }
}
