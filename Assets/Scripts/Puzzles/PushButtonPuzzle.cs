using UnityEngine;

public class PushButtonPuzzle : Switch_Basic
{
    [SerializeField] private int id;
    [SerializeField] private PushButtonPuzzleCenter central;

    private void OnValidate()
    {
        name = $"PushButton [{id}]";
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isToggled) return;

        SetToggle(true);
        central.RegisterButton(this);
    }
}