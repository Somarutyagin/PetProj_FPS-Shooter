using UnityEngine;

public interface IInputProvider
{
    Vector2 GetMovementInput();
    Vector2 GetLookInput();
    bool IsJumpPressed();
    bool IsCrouchPressed();
    bool IsRunPressed();
    bool IsFirePressed();
    bool IsReloadPressed();
}
