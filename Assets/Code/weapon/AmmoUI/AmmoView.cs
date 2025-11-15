using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using Cysharp.Threading.Tasks;

public class AmmoView : MonoBehaviour
{
    [Inject] private WeaponController _weaponController;
    [SerializeField] private Text ammoText;
    [SerializeField] private Image ammoBar;
    [SerializeField] private Image ammoDelta;

    private AmmoViewModel _currentViewModel;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    private const int _iterationCount = 30;
    private const float _animTime = 0.2f;
    private float previousAmmo;

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

        _currentViewModel.Model.CurrentAmmo
            .CombineLatest(_currentViewModel.Model.MaxAmmo, (current, max) => (current, max))
            .Subscribe(UpdateUI)
            .AddTo(_disposables);

        InstantValueAmmoBar();
        previousAmmo = _currentViewModel.Model.CurrentAmmo.Value;
    }

    private void InstantValueAmmoBar()
    {
        ammoText.text = $"{_currentViewModel.Model.CurrentAmmo.Value}/{_currentViewModel.Model.MaxAmmo.Value}";

        if (ammoBar != null)
        {
            ammoBar.fillAmount = (float)_currentViewModel.Model.CurrentAmmo.Value / _currentViewModel.Model.MaxAmmo.Value;
        }
        if (ammoDelta != null)
        {
            ammoDelta.fillAmount = (float)_currentViewModel.Model.CurrentAmmo.Value / _currentViewModel.Model.MaxAmmo.Value;
        }
    }

    private void UpdateUI((int current, int max) ammo)
    {
        ammoText.text = $"{ammo.current}/{ammo.max}";

        float currentFill = (float)ammo.current / ammo.max;
        float previousFill = (float)previousAmmo / ammo.max;

        if (previousAmmo < ammo.current)
        {
            ammoDelta.fillAmount = currentFill;
            AnimateAmmoBarDeltaAsync(true, currentFill - previousFill).Forget();
        }
        else
        {
            ammoBar.fillAmount = currentFill;
            AnimateAmmoBarDeltaAsync(false, previousFill - currentFill).Forget();
        }

        previousAmmo = ammo.current;
    }

    private async UniTask AnimateAmmoBarDeltaAsync(bool less, float delta)
    {
        float step = delta / _iterationCount;
        float delay = _animTime / _iterationCount;

        for (int i = 0; i < _iterationCount; i++)
        {
            if (less)
            {
                ammoBar.fillAmount += step;
            }
            else
            {
                ammoDelta.fillAmount -= step;
            }
            await UniTask.Delay((int)(delay * 1000), DelayType.DeltaTime);
        }
    }
}