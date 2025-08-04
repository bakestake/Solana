using UnityEngine;

namespace Bakeland
{
    public class PlayerMovementDisabler : MonoBehaviour
    {
        public void EnablePlayer()
        {
            PlayerController.canMove = true;
        }

        public void DisablePlayer()
        {
            PlayerController.canMove = false;
        }
    }
}
