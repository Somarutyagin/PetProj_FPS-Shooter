using Zenject;
using UniRx;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private Text _speedText;
    [SerializeField] private Text _armorText;
    [SerializeField] private Text _lifestealText;

    private StatsViewModel _viewModel;

    [Inject]
    public void Construct(StatsViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    private void Start()
    {
        _viewModel.GetSpeedValue().Subscribe(value => UpdateText(_speedText, value)).AddTo(this);
        _viewModel.GetArmorValue().Subscribe(value => UpdateText(_armorText, value)).AddTo(this);
        _viewModel.GetVampirismValue().Subscribe(value => UpdateText(_lifestealText, value)).AddTo(this);
    }

    private void UpdateText(Text textComponent, float value)
    {
        textComponent.text = NumberFormatter.ConvertPercentToScaleAndFormat(value);

        textComponent.transform.DOScale(1.2f, 0.2f).OnComplete(() => textComponent.transform.DOScale(1f, 0.2f));
    }
}