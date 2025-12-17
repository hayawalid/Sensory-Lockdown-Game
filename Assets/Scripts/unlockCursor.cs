using UnityEngine;

public class UnlockCursor : MonoBehaviour
{
    void LateUpdate()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
