using UnityEngine;
using Zenject;
using System.Collections.Generic;

[RequireComponent(typeof(KillCounter))]
public class EnemySpawner : MonoBehaviour
{
    [Inject] private EnemySpawnConfig _enemySpawnConfig;
    [Inject] private EnemyConfig enemyConfig;
    [Inject] private StatsManager statsManager;
    [Inject] private EnemyHealthFactory _healthFactory;

    private KillCounter counter;
    public ObjectPool<EnemyAI> EnemyPool { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemyTransformPool;
    [SerializeField] private LayerMask groundLayer;

    private int currentActiveEnemies = 0;
    private float lastSpawnTime;
    private float _enemyHeight; // Height of the enemy's collider for proper placement

    private Dictionary<EnemyAI, EnemyHealthComponents> _healthComponents = new Dictionary<EnemyAI, EnemyHealthComponents>();

    private void Awake()
    {
        counter = GetComponent<KillCounter>();

        if (enemyPrefab != null)
        {
            EnemyPool = new ObjectPool<EnemyAI>(enemyPrefab, _enemySpawnConfig.PoolSize, enemyTransformPool);

            // Get enemy height from collider (assuming CapsuleCollider; adjust if different)
            var collider = enemyPrefab.GetComponent<CapsuleCollider>();
            if (collider != null)
            {
                _enemyHeight = collider.height;
            }
            else
            {
                Debug.LogWarning("Enemy prefab does not have a CapsuleCollider. Using default height of 2f.");
                _enemyHeight = 2f; // Default fallback
            }
        }
    }
    private void Start()
    {
        lastSpawnTime = Time.time;
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned in EnemySpawner!");
        }
    }

    private void Update()
    {
        if (Time.time >= lastSpawnTime + _enemySpawnConfig.SpawnInterval && currentActiveEnemies < _enemySpawnConfig.MaxEnemies && EnemyPool != null)
        {
            SpawnEnemy();
            lastSpawnTime = Time.time;
        }
    }

    private void SpawnEnemy()
    {
        EnemyAI enemy = EnemyPool.Get();
        var healthComponents = _healthFactory.Create(enemy);
        enemy.Initialize(this, player, healthComponents.ViewModel, statsManager, enemyConfig);
        _healthComponents[enemy] = healthComponents;

        Vector3 spawnPosition = GetSpawnPosition();
        enemy.transform.position = spawnPosition;
        enemy.transform.rotation = Quaternion.identity;

        enemy.ResetForPool();
        enemy.gameObject.SetActive(true);

        currentActiveEnemies++;

        enemy.OnEnemyDeath += OnEnemyKilled;
    }

    private Vector3 GetSpawnPosition()
    {
        if (player != null)
        {
            Vector3 grounded;
            if (TryGetGroundedSpawnAroundPlayer(out grounded))
            {
                return grounded;
            }

            // Fallback: spawn near player, using raycast from above to find ground height
            Vector3 fallbackPos = player.position + new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
            if (TryGetGroundHeightAtPosition(fallbackPos, out float groundY))
            {
                return new Vector3(fallbackPos.x, groundY + _enemyHeight / 2, fallbackPos.z);
            }
            else
            {
                // Ultimate fallback with default y
                return new Vector3(fallbackPos.x, 1f + _enemyHeight / 2, fallbackPos.z);
            }
        }
        return transform.position;
    }

    private bool TryGetGroundedSpawnAroundPlayer(out Vector3 groundedPosition)
    {
        groundedPosition = Vector3.zero;
        if (player == null)
        {
            Debug.LogWarning("Player is null, cannot spawn around player.");
            return false;
        }
        int layerMask = groundLayer.value == 0 ? Physics.DefaultRaycastLayers : groundLayer.value;
        float maxDownDistance = _enemySpawnConfig.GroundCheckHeight + _enemySpawnConfig.GroundCheckDistance;
        for (int attempt = 0; attempt < _enemySpawnConfig.SpawnPositionTries; attempt++)
        {
            float radius = Random.Range(_enemySpawnConfig.MinSpawnRadius, _enemySpawnConfig.MaxSpawnRadius);
            float angle = Random.Range(0f, Mathf.PI * 2f);
            Vector3 ringOffset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            Vector3 sampleOrigin = player.position + ringOffset + Vector3.up * _enemySpawnConfig.GroundCheckHeight;
            Debug.DrawRay(sampleOrigin, Vector3.down * maxDownDistance, Color.red, 1f);
            if (Physics.Raycast(sampleOrigin, Vector3.down, out RaycastHit hit, maxDownDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                // Set y to ground height + half enemy height to place enemy on surface
                groundedPosition = new Vector3(hit.point.x, hit.point.y + _enemyHeight / 2, hit.point.z);
                return true;
            }
        }
        Debug.LogWarning("Failed to find grounded spawn position after all attempts.");
        return false;
    }

    private bool TryGetGroundHeightAtPosition(Vector3 position, out float groundY)
    {
        groundY = 0f;
        int layerMask = groundLayer.value == 0 ? Physics.DefaultRaycastLayers : groundLayer.value;
        float maxDownDistance = _enemySpawnConfig.GroundCheckHeight + _enemySpawnConfig.GroundCheckDistance;
        Vector3 sampleOrigin = position + Vector3.up * _enemySpawnConfig.GroundCheckHeight;
        if (Physics.Raycast(sampleOrigin, Vector3.down, out RaycastHit hit, maxDownDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            groundY = hit.point.y;
            return true;
        }
        return false;
    }

    public void OnEnemyKilled(EnemyAI enemyAI)
    {
        counter.UpdateKillUI();
        currentActiveEnemies--;
    }

    private void OnDestroy()
    {
        EnemyPool?.Clear();
        _healthComponents.Clear();
    }
}