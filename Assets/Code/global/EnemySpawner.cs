using UnityEngine;

[RequireComponent(typeof(KillCounter))]
public class EnemySpawner : MonoBehaviour
{
    public ObjectPool<EnemyAI> EnemyPool { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
	[SerializeField] private Transform player;
    [SerializeField] private Transform enemyTransformPool;
	[SerializeField] private LayerMask groundLayer;
	private const float minSpawnRadius = 10f;
	private const float maxSpawnRadius = 20f;
	private const int spawnPositionTries = 8;
	private const float groundCheckHeight = 5f;
	private const float groundCheckDistance = 20f;

    private KillCounter counter;

    private const float spawnInterval = 5f;
    private const int maxEnemies = 50;
    private const int poolSize = 100;

    private int currentActiveEnemies = 0;
    private float lastSpawnTime;
    private void Awake()
    {
        counter = GetComponent<KillCounter>();

        if (enemyPrefab != null)
        {
            EnemyPool = new ObjectPool<EnemyAI>(enemyPrefab, poolSize, enemyTransformPool);
        }
    }

    private void Start()
    {
        lastSpawnTime = Time.time;
    }
    private void Update()
    {
        if (Time.time >= lastSpawnTime + spawnInterval && currentActiveEnemies < maxEnemies && EnemyPool != null)
        {
            SpawnEnemy();
            lastSpawnTime = Time.time;
        }
    }
    private void SpawnEnemy()
    {
		EnemyAI enemy = EnemyPool.Get();
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
		}

		return transform.position;
	}

	private bool TryGetGroundedSpawnAroundPlayer(out Vector3 groundedPosition)
	{
		groundedPosition = Vector3.zero;
		if (player == null)
		{
			return false;
		}

		int layerMask = groundLayer.value == 0 ? Physics.DefaultRaycastLayers : groundLayer.value;
		float maxDownDistance = groundCheckHeight + groundCheckDistance;

		for (int attempt = 0; attempt < spawnPositionTries; attempt++)
		{
			float radius = Random.Range(minSpawnRadius, maxSpawnRadius);
			float angle = Random.Range(0f, Mathf.PI * 2f);
			Vector3 ringOffset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
			Vector3 sampleOrigin = player.position + ringOffset + Vector3.up * groundCheckHeight;

			if (Physics.Raycast(sampleOrigin, Vector3.down, out RaycastHit hit, maxDownDistance, layerMask, QueryTriggerInteraction.Ignore))
			{
				groundedPosition = hit.point;
				return true;
			}
		}

		return false;
	}
    public void OnEnemyKilled()
    {
        counter.UpdateKillUI();
        currentActiveEnemies--;
    }
    private void OnDestroy()
    {
        EnemyPool?.Clear();
    }
}
