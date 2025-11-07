using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text ammoText;

    private IWeapon currentWeapon;
    private IInputProvider inputProvider;

    private Camera playerCamera;

    private const float aimingFov = 40f;
    private const float normalFov = 60f;

    private void Awake()
    {
        currentWeapon = GetComponentInChildren<IWeapon>();
        inputProvider = GetComponent<IInputProvider>();
        if (ammoText != null) ammoText.text = currentWeapon.GetAmmo().ToString();

        playerCamera = Camera.main;
    }

    private void Update()
    {
        HandleFire();
        HandleReload();
        HandleADS();
        UpdateUI();
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
        if (inputProvider.IsReloadPressed() && currentWeapon.GetAmmo() < 30)
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

    private void UpdateUI()
    {
        string result = currentWeapon.GetAmmo().ToString() + "/" + currentWeapon.GetMaxAmmoInfo().ToString();
        if (ammoText != null && ammoText.text != result) ammoText.text = result;
    }
}
