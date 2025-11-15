using UnityEngine;
using Zenject;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class EnemyHealthFactory : IFactory<EnemyAI, EnemyHealthComponents>
{
    private readonly DiContainer _container;

    public EnemyHealthFactory(DiContainer container)
    {
        _container = container;
    }
    public EnemyHealthComponents Create(EnemyAI enemy)
    {
        var subContainer = _container.CreateSubContainer();

        subContainer.Bind<IHealthModel>().To<HealthModel>().AsTransient().WithArguments(100f);
        subContainer.Bind<IHealthViewModel>().To<HealthViewModel>().AsTransient();

        if (enemy.TryGetComponent(out HealthView view))
        {
            view.ViewModel = subContainer.Resolve<IHealthViewModel>();

            view.ViewModel = new HealthViewModel(new HealthModel(100));

            view.Initialize();

            return new EnemyHealthComponents
            {
                View = view,
                ViewModel = view.ViewModel,
                Model = view.ViewModel.Model,
                Enemy = enemy
            };
        }
        else
        {
            Debug.LogError("HealthView not found on enemy prefab!");
            return null;
        }
    }

    public void Despawn(EnemyHealthComponents components)
    {
        components.ViewModel?.Dispose();
    }
}