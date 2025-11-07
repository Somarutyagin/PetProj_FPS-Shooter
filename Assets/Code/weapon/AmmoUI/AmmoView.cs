using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AmmoView : MonoBehaviour
{
    [Inject] private AmmoViewModel viewModel;
    [SerializeField] private Text ammoText;

    private void Start()
    {
        viewModel.OnAmmoChanged += UpdateUI;
    }

    private void OnDisable()
    {
        viewModel.OnAmmoChanged -= UpdateUI;
    }

    private void UpdateUI(int current, int max)
    {
        ammoText.text = $"{current}/{max}";
    }
}