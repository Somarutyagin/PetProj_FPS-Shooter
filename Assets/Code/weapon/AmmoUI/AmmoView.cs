using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AmmoView : MonoBehaviour
{
    [Inject] private WeaponController _weaponController;
    [SerializeField] private Text ammoText;
    
    private AmmoViewModel _currentViewModel;
    
    private void Start()
    {
        _weaponController.OnActiveWeaponChanged += OnWeaponChanged;
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
        UpdateUI(_currentViewModel.model.CurrentAmmo, _currentViewModel.model.MaxAmmo);
    }
    
    private void UpdateUI(int current, int max)
    {
        ammoText.text = $"{current}/{max}";
    }
}