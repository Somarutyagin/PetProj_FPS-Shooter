using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AmmoView : MonoBehaviour
{
    [Inject] private WeaponController _weaponController;
    [SerializeField] private Text ammoText;
    [SerializeField] private Image ammoBar;
    [SerializeField] private Image ammoDelta;
    
    private AmmoViewModel _currentViewModel;

    private const int _iterationCount = 30;
    private const float _animTime = 0.2f;
    private float previousAmmo;
    private float _timerA = 0f;
    private int _currentIterationA = 0;
    private bool _isAnimatingA = false;
    private float _targetFillA;


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
        if (_currentViewModel != null)
            _currentViewModel.OnAmmoChanged -= UpdateUI;
        _weaponController.OnActiveWeaponChanged -= OnWeaponChanged;
    }
    
    private void OnWeaponChanged(AmmoViewModel viewModel)
    {
        if (_currentViewModel != null)
            _currentViewModel.OnAmmoChanged -= UpdateUI;
        
        _currentViewModel = viewModel;
        _currentViewModel.OnAmmoChanged += UpdateUI;
        InstantValueAmmoBar();

        previousAmmo = _currentViewModel.model.CurrentAmmo;
    }
    private void InstantValueAmmoBar()
    {
        ammoText.text = $"{_currentViewModel.model.CurrentAmmo}/{_currentViewModel.model.MaxAmmo}";

        if (ammoBar != null)
        {
            ammoBar.fillAmount = (float)_currentViewModel.model.CurrentAmmo / _currentViewModel.model.MaxAmmo;
        }
        if (ammoDelta != null)
        {
            ammoDelta.fillAmount = (float)_currentViewModel.model.CurrentAmmo / _currentViewModel.model.MaxAmmo;
        }
    }

    private void UpdateUI(int current, int max)
    {
        ammoText.text = $"{current}/{max}";

        if (previousAmmo < current)
        {
            ammoDelta.fillAmount = (float)current / max;
            StartAmmoBarDelta(true);
        }
        else
        {
            ammoBar.fillAmount = (float)current / max;
            StartAmmoBarDelta(false);
        }
    }
    private void StartAmmoBarDelta(bool less)
    {
        _isAnimatingA = true;
        _currentIterationA = 0;
        _timerA = 0f;

        if (less)
        {
            _targetFillA = ammoDelta.fillAmount - ammoBar.fillAmount;
        }
        else
        {
            _targetFillA = ammoBar.fillAmount - ammoDelta.fillAmount;
        }
    }
    private void FixedUpdate()
    {
        if (_isAnimatingA)
        {
            _timerA += Time.fixedDeltaTime;
            if (_currentIterationA < _iterationCount)
            {
                if (_timerA >= (_animTime / _iterationCount))
                {
                    _timerA -= (_animTime / _iterationCount);
                    _currentIterationA++;

                    if (previousAmmo < _currentViewModel.model.CurrentAmmo)
                    {
                        ammoBar.fillAmount += _targetFillA / _iterationCount;
                    }
                    else
                    {
                        ammoDelta.fillAmount += _targetFillA / _iterationCount;
                    }
                }
            }
            else
            {
                _isAnimatingA = false;
                previousAmmo = _currentViewModel.model.CurrentAmmo;
            }
        }
    }
}