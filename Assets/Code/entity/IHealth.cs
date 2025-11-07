public interface IHealth
{
    void SetHealth(float health);
    void TakeDamage(float damage);
    bool IsDead();
}