[System.Serializable]
public class PlayerInteractionCondition : IInteractionCondition
{
    public bool CanInteract()
    {
        return PlayerController.canInteract && PlayerController.canMove;
    }

    public void OnInteracted()
    {

    }
}