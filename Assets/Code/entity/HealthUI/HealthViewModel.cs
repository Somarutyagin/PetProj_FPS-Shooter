using UniRx;
using Cysharp.Threading.Tasks;
using System.Diagnostics;

public class HealthViewModel : IHealthViewModel
{
    public IHealthModel Model { get; }
    public ReactiveCommand OnDeath { get; } = new ReactiveCommand();
    public ReactiveCommand<float> TakeDamageCommand { get; } = new ReactiveCommand<float>();
    public ReactiveCommand<float> SetHealthCommand { get; } = new ReactiveCommand<float>();
    public bool IsDead => Model.CurrentHealth.Value <= 0;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    public HealthViewModel(IHealthModel model)
    {
        Model = model;

        TakeDamageCommand.Subscribe(damage => TakeDamage(damage)).AddTo(_disposables);
        SetHealthCommand.Subscribe(health => Model.SetHealth(health)).AddTo(_disposables);

        Model.CurrentHealth.Subscribe(health =>
        {
            if (health <= 0)
            {
                OnDeath.Execute();
            }
        }).AddTo(_disposables);
    }
    private void TakeDamage(float damage)
    {
        float newHealth = Model.CurrentHealth.Value - damage;
        Model.SetHealth(newHealth);
    }
    public void Dispose()
    {
        _disposables.Dispose();
    }
}