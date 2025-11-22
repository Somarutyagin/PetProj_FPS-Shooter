using System.Collections.Generic;
using UniRx;
using Zenject;

public class ExperienceViewModel
{
    public IReadOnlyReactiveProperty<int> CurrentLevel { get; }
    public IReadOnlyReactiveProperty<float> ExpFillAmount { get; }

    private readonly ExperienceManager _experienceManager;
    private readonly ExperienceConfig _config;

    public ExperienceViewModel(ExperienceManager experienceManager, ExperienceConfig config)
    {
        _config = config;
        _experienceManager = experienceManager;

        CurrentLevel = _experienceManager.CurrentLevel;

        ExpFillAmount = _experienceManager.CurrentExp
            .Select(exp => exp / _config.ExpPerLevel)
            .ToReadOnlyReactiveProperty();
    }
}