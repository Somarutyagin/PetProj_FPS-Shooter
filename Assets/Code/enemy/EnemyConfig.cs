using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public float MoveSpeed = 1f;
    public float DetectionRange = 60f;
    public float AttackTime = 0.5f;
    public float Damage = 10f;
}