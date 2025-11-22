using System;
using UniRx;
using Zenject;

public class ExperienceManager : IInitializable
{
    [Inject] ExperienceConfig experienceConfig { get; set; }

    private readonly ReactiveProperty<int> _currentLevel = new ReactiveProperty<int>(1);
    private readonly ReactiveProperty<float> _currentExp = new ReactiveProperty<float>(0f);
    private readonly Subject<Unit> _onLevelUp = new Subject<Unit>();

    public IReadOnlyReactiveProperty<int> CurrentLevel => _currentLevel;
    public IReadOnlyReactiveProperty<float> CurrentExp => _currentExp;
    public IObservable<Unit> OnLevelUp => _onLevelUp;

    [Inject]
    public void Initialize()
    {
        experienceConfig.ExpPerLevel = experienceConfig.BaseExpPerLevel;
    }

    public void AddExperience(float amount)
    {
        _currentExp.Value += amount;

        while (_currentExp.Value >= experienceConfig.ExpPerLevel)
        {
            float expRequiredForCurrentLevel = experienceConfig.ExpPerLevel; // Store the current required exp before scaling

            _currentExp.Value -= expRequiredForCurrentLevel;
            _currentLevel.Value++;
            _onLevelUp.OnNext(Unit.Default);

            experienceConfig.ExpPerLevel *= experienceConfig.ExpScalerPerLevel; // Scale for the next level
        }
    }

}