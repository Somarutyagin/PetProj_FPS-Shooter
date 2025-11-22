using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

public class AmmoView : MonoBehaviour
{
    [Inject] private WeaponController _weaponController;
    [SerializeField] private Text ammoText;
    [SerializeField] private Image ammoBar;
    [SerializeField] private Image ammoDelta;

    private AmmoViewModel _currentViewModel;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    private float previousAmmo = 0f;

    private void Start()
    {
        InitializeAmmoBar();
        _weaponController.OnActiveWeaponChanged += OnWeaponChanged;
    }

    private void InitializeAmmoBar()
    {
        if (ammoBar != null)
        {
            ammoBar.fillAmount = 1f;
        }
        if (ammoDelta != null)
        {
            ammoDelta.fillAmount = 1f;
        }
    }

    private void OnDisable()
    {
        _disposables.Clear();
        _weaponController.OnActiveWeaponChanged -= OnWeaponChanged;
    }

    private void OnWeaponChanged(AmmoViewModel viewModel)
    {
        _disposables.Clear();

        _currentViewModel = viewModel;

        if (previousAmmo == 0)
        {
            previousAmmo = _currentViewModel.Model.CurrentAmmo.Value;
        }

        _currentViewModel.Model.CurrentAmmo
            .Subscribe(ammo =>
            {
                float fill = (float)ammo / _currentViewModel.Model.MaxAmmo.Value;

                if (previousAmmo >= ammo)
                {
                    ammoBar.fillAmount = fill;
                    ammoDelta.DOFillAmount(fill, 0.2f); // Animate fill amount with DOTween
                }
                else
                {
                    ammoDelta.fillAmount = fill;
                    ammoBar.DOFillAmount(fill, 0.2f); // Animate fill amount with DOTween
                }
                previousAmmo = ammo;
                ammoText.text = $"{ammo}/{_currentViewModel.Model.MaxAmmo.Value}";
            })
            .AddTo(_disposables);

        previousAmmo = _currentViewModel.Model.CurrentAmmo.Value;
    }
}