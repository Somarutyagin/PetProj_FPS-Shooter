using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnConfig", menuName = "Scriptable Objects/EnemySpawnConfig")]
public class EnemySpawnConfig : ScriptableObject
{
    public float MinSpawnRadius = 10f;
    public float MaxSpawnRadius = 20f;
    public int SpawnPositionTries = 8;
    public float GroundCheckHeight = 5f;
    public float GroundCheckDistance = 20f;
    public float SpawnInterval = 2f;
    public int MaxEnemies = 50;
    public int PoolSize = 100;
}