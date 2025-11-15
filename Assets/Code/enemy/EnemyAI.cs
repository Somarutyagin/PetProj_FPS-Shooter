using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthView))]
public class EnemyAI : MonoBehaviour
{
    public IHealthViewModel HealthViewModel { get; set; }

    [SerializeField] private Transform player;

    private const float moveSpeed = 1f;
    private const float detectionRange = 30f;

    private EnemySpawner spawner;
    private Rigidbody rb;
    private bool isDead = false;
    private bool isPooled = false;

    public event Action<EnemyAI> OnEnemyDeath;

    private const float attackTime = 0.5f;
    private float attackTimer = 0f;

    [Obsolete]
    private void Awake()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    [Obsolete]
    private void OnDestroy()
    {
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
        attackTimer += Time.deltaTime;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (attackTimer >= attackTime && !isDead && collision.transform.TryGetComponent(out PlayerHealthHandler playerHealth))
        {
            playerHealth.TakeDamage(10f);

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
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }
    private void ReturnToPool()
    {
        spawner.EnemyPool.Return(this);
        gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        HealthViewModel?.TakeDamageCommand.Execute(damage);
    }
}
