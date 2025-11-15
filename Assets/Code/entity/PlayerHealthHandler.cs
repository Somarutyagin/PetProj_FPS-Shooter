using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class PlayerHealthHandler : MonoBehaviour
{
    [Inject] private IHealthViewModel ViewModel { get; set; }

    private void Start()
    {
        HealthView view = GetComponent<HealthView>();

        view.ViewModel = ViewModel;

        view.Initialize();

        ViewModel.OnDeath.Subscribe(_ => RestartScene()).AddTo(this);
    }

    private async void RestartScene()
    {
        await UniTask.Delay(2000);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void TakeDamage(float damage)
    {
        ViewModel?.TakeDamageCommand.Execute(damage);
    }
}