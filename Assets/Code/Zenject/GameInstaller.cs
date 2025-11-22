using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<WeaponConfigsContainer>().FromScriptableObjectResource("Configs/Weapons/WeaponConfigsContainer").AsSingle();
        Container.Bind<PlayerStatsConfig>().FromScriptableObjectResource("Configs/Stats/PlayerStatsConfig").AsSingle();
        Container.Bind<ExperienceConfig>().FromScriptableObjectResource("Configs/Experience/ExperienceConfig").AsSingle();
        Container.Bind<PlayerConfig>().FromScriptableObjectResource("Configs/Player/PlayerConfig").AsSingle();
        Container.Bind<EnemyConfig>().FromScriptableObjectResource("Configs/Enemy/EnemyConfig").AsSingle();
        Container.Bind<EnemySpawnConfig>().FromScriptableObjectResource("Configs/Enemy/EnemySpawnConfig").AsSingle();

        Container.Bind<GameObject>().WithId("BulletHole").FromResource("Prefabs/BulletHole").AsCached();
        Container.Bind<GameObject>().WithId("BulletTracer").FromResource("Prefabs/BulletTracer").AsCached();

        Container.Bind<AmmoModel>().AsTransient();
        Container.Bind<WeaponController>().FromComponentInHierarchy().AsSingle();

        Container.Bind<WeaponFactory>().AsSingle();
        Container.Bind<AmmoViewModelFactory>().AsSingle();

        Container.Bind<AmmoViewModel>().AsTransient();
        Container.Bind<AmmoView>().AsSingle();

        Container.Bind<IHealthModel>().FromInstance(new HealthModel(100)).AsSingle();
        Container.Bind<IHealthViewModel>().To<HealthViewModel>().AsSingle();

        Container.Bind<EnemyHealthFactory>().AsSingle();

        Container.Bind<RarityManager>().AsSingle();
        Container.Bind<StatsManager>().AsSingle();
        Container.Bind<ExperienceManager>().AsSingle();
        Container.Bind<UpgradeStatCommand>().AsSingle();
        Container.Bind<StatsViewModel>().AsSingle();
        Container.Bind<ExperienceViewModel>().AsSingle();

        Container.Bind<PlayerHealthHandler>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameStateManager>().FromComponentInHierarchy().AsSingle();
    }
}