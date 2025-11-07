using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<AmmoModel>().AsSingle().WithArguments(30);

        Container.Bind<AmmoViewModel>().AsSingle();
    }
}