using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ExperienceView : MonoBehaviour
{
    [SerializeField] private Image _expBar;
    [SerializeField] private Image _expDelta;
    [SerializeField] private Text _levelTxt;

    private ExperienceViewModel _viewModel;
    private CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public void Construct(ExperienceViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    private void Start()
    {
        _viewModel.ExpFillAmount
            .Subscribe(fillAmount =>
            {
                _expDelta.fillAmount = fillAmount;
                _expBar.DOFillAmount(fillAmount, 0.2f); // Animate fill amount with DOTween
            })
            .AddTo(_disposables);

        _viewModel.CurrentLevel
            .Subscribe(level =>
            {
                _levelTxt.text = $"Lvl {level}";
            })
            .AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}