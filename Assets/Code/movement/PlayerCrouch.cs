using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(IInputProvider))]
public class PlayerCrouch : MonoBehaviour
{
    private const float crouchHeight = 1f;
    private const float standHeight = 2f;
    private readonly Vector3 crouchCenter = new Vector3(0f, -0.5f, 0f);
    private readonly Vector3 standCenter = new Vector3(0f, 0f, 0f);

    private CapsuleCollider capsuleCollider;
    private IInputProvider inputProvider;
    private Camera playerCamera;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        inputProvider = GetComponent<IInputProvider>();
        playerCamera = GetComponentInChildren<Camera>();
        capsuleCollider.height = standHeight;
    }

    private void Update()
    {
        bool isCrouching = inputProvider.IsCrouchPressed();

        capsuleCollider.height = isCrouching ? crouchHeight : standHeight;
        capsuleCollider.center = isCrouching ? crouchCenter : standCenter;
        playerCamera.transform.localPosition = new Vector3(0, isCrouching ? 0f : 0.5f, 0);
    }
}
