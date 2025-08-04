using UnityEngine;

public class PlayerEnabler : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private bool _enable;

    public bool Enable
    {
        get { return _enable; }
        set
        {
            if (_enable != value)  // Only call the function if the value is changing
            {
                _enable = value;
                OnEnableToggled();  // Call the function when the value changes
            }
        }
    }

    void OnEnableToggled()
    {
        if (player)
        {
            PlayerController.canMove = _enable;
        }
    }

    void Update()
    {
        PlayerController.canMove = _enable;
    }
}
