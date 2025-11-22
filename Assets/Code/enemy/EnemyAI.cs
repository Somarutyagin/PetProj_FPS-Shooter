using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthView))]
public class EnemyAI : MonoBehaviour
{
    private IHealthViewModel _healthViewModel;

    private Transform _player;
    private EnemySpawner _spawner;
    private StatsManager _statsManager;
    private EnemyConfig _enemyConfig;

    private float attackTimer = 0f;

    private Rigidbody rb;
    private bool isDead = false;
    private bool isPooled = false;

    public event Action<EnemyAI> OnEnemyDeath;

    public void Initialize(EnemySpawner spawner, Transform player, IHealthViewModel healthViewModel, StatsManager statsManager, EnemyConfig config)
    {
        _spawner = spawner;
        _player = player;
        _statsManager = statsManager;
        _healthViewModel = healthViewModel;
        _enemyConfig = config;

        rb = GetComponent<Rigidbody>();
    }

    [Obsolete]
    private void OnDestroy()
    {
        if (_spawner != null) OnEnemyDeath -= _spawner.OnEnemyKilled;
    }

    private void FixedUpdate()
    {
        if (isDead || _player == null || !isPooled) return;
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        if (distanceToPlayer <= _enemyConfig.DetectionRange)
        {
            Vector3 directionToPlayer = (_player.position - transform.position).normalized;
            directionToPlayer = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
            rb.linearVelocity = new Vector3(directionToPlayer.x * _enemyConfig.MoveSpeed, rb.linearVelocity.y, directionToPlayer.z * _enemyConfig.MoveSpeed);
            transform.LookAt(new Vector3(_player.position.x, transform.position.y, _player.position.z));
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
    private void Update()
    {
        attackTimer += Time.deltaTime;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (attackTimer >= _enemyConfig.AttackTime && !isDead && collision.transform.TryGetComponent(out PlayerHealthHandler playerHealth))
        {
            playerHealth.TakeDamage(_enemyConfig.Damage - (_statsManager.GetStatPrecent(StatType.Armor).Value / 100f) * _enemyConfig.Damage);

            attackTimer = 0f;
        }
    }

    public void OnEnemyDeathHandler()
    {
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        OnEnemyDeath?.Invoke(this);

        Invoke(nameof(ReturnToPool), 0.5f);
    }
    public void ResetForPool()
    {
        isDead = false;
        isPooled = true;
        rb.linearVelocity = Vector3.zero;
    }
    private void ReturnToPool()
    {
        _spawner.EnemyPool.Return(this);
        gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        _healthViewModel?.TakeDamageCommand.Execute(damage);
    }
}
