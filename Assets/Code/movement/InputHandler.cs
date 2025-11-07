using UnityEngine;

public class InputHandler : MonoBehaviour, IInputProvider
{
    public Vector2 GetMovementInput()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public Vector2 GetLookInput()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public bool IsJumpPressed() => Input.GetKeyDown(KeyCode.Space);
    public bool IsCrouchPressed() => Input.GetKey(KeyCode.LeftControl);
    public bool IsRunPressed() => Input.GetKey(KeyCode.LeftShift);
    public bool IsFirePressed() => Input.GetMouseButton(0);
    public bool IsReloadPressed() => Input.GetKeyDown(KeyCode.R);
}
