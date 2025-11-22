using UnityEngine;

[CreateAssetMenu(fileName = "ExperienceConfig", menuName = "Scriptable Objects/ExperienceConfig")]
public class ExperienceConfig : ScriptableObject
{
    public float BaseExpPerLevel = 100;
    public float ExpPerLevel;
    public float ExpScalerPerLevel;
    public float ExpForEnemy;
}
