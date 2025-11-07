using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private IWeapon currentWeapon;
    private IInputProvider inputProvider;

    private Camera playerCamera;

    private const float aimingFov = 40f;
    private const float normalFov = 60f;

    private void Awake()
    {
        currentWeapon = GetComponentInChildren<IWeapon>();
        inputProvider = GetComponent<IInputProvider>();

        playerCamera = Camera.main;
    }

    private void Update()
    {
        HandleFire();
        HandleReload();
        HandleADS();
    }

    private void HandleFire()
    {
        if (inputProvider.IsFirePressed() && currentWeapon.CanFire())
        {
            currentWeapon.Fire();
        }
    }

    private void HandleReload()
    {
        if (inputProvider.IsReloadPressed())
        {
            currentWeapon.Reload();
        }
    }

    private void HandleADS()
    {
        bool isAiming = Input.GetMouseButton(1);

        float targetFOV = isAiming ? aimingFov : normalFov;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * 5f);

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            if (transform.GetChild(i).GetChild(i).CompareTag("Weapon"))
            {
                Vector3 targetWeaponPos = isAiming ? new Vector3(0, -0.5f, 1f) : new Vector3(0.5f, -0.7f, 1f);
                transform.GetChild(i).GetChild(i).localPosition = Vector3.Lerp(transform.GetChild(i).GetChild(i).localPosition, targetWeaponPos, Time.deltaTime * 5f);
            }
        }
    }
}
