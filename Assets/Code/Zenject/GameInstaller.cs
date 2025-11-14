using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<WeaponConfigsContainer>().FromScriptableObjectResource("Configs/Weapons/WeaponConfigsContainer").AsSingle();
        Container.Bind<GameObject>().WithId("BulletHole").FromResource("Prefabs/BulletHole").AsCached();
        Container.Bind<GameObject>().WithId("BulletTracer").FromResource("Prefabs/BulletTracer").AsCached();

        Container.Bind<AmmoModel>().AsTransient();
        Container.Bind<WeaponController>().FromComponentInHierarchy().AsSingle();

        Container.Bind<WeaponFactory>().AsSingle();
        Container.Bind<AmmoViewModelFactory>().AsSingle();

        Container.Bind<AmmoViewModel>().AsTransient();
        Container.Bind<AmmoView>().AsSingle();
    }
}