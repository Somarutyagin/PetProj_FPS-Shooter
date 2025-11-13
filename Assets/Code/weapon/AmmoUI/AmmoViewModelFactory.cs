using Zenject;

public class AmmoViewModelFactory : IFactory<Weapon, AmmoViewModel>
{
    private readonly DiContainer _container;
    
    public AmmoViewModelFactory(DiContainer container)
    {
        _container = container;
    }
    
    public AmmoViewModel Create(Weapon weapon)
    {
        if (weapon == null) throw new System.ArgumentNullException(nameof(weapon));
        
        return _container.Instantiate<AmmoViewModel>(new object[] { weapon });
    }
}