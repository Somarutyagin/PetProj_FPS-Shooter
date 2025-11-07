using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform player;

    private const float moveSpeed = 1f;
    private const float detectionRange = 30f;

    private EnemySpawner spawner;
    private Rigidbody rb;
    private Health health;
    private bool isDead = false;
    private bool isPooled = false;

    public event Action OnEnemyDeath;

    [Obsolete]
    private void Awake()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        health.OnDeath += OnEnemyDeathHandler;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    [Obsolete]
    private void OnDestroy()
    {
        if (health != null) health.OnDeath -= OnEnemyDeathHandler;
        if (spawner != null) OnEnemyDeath -= spawner.OnEnemyKilled;
    }

    private void FixedUpdate()
    {
        if (isDead || player == null || !isPooled) return;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
            rb.linearVelocity = new Vector3(directionToPlayer.x * moveSpeed, rb.linearVelocity.y, directionToPlayer.z * moveSpeed);
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
    private void Update()
    {
        if (!isDead && player != null && Vector3.Distance(transform.position, player.position) < 1.5f)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null) playerHealth.TakeDamage(10f * Time.deltaTime);
        }
    }

    private void OnEnemyDeathHandler()
    {
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        OnEnemyDeath?.Invoke();

        Invoke(nameof(ReturnToPool), 0.5f);
    }
    public void ResetForPool()
    {
        isDead = false;
        isPooled = true;
        transform.position = Vector3.zero;
        health.SetHealth(health.MaxHealth);
        rb.linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }
    private void ReturnToPool()
    {
        spawner.EnemyPool.Return(this);
        gameObject.SetActive(false);
    }
}
