using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class KillCounter : MonoBehaviour
{
    [Inject] ExperienceConfig experienceConfig { get; set; }
    [Inject] ExperienceManager experienceManager { get; set; }
    [SerializeField] private Text killCountText;

    private int killCount = 0;

    private void Awake()
    {
        if (killCountText != null) killCountText.text = "Kills: 0";
    }
    public void UpdateKillUI()
    {
        experienceManager.AddExperience(experienceConfig.ExpForEnemy);

        killCount++;
        if (killCountText != null && killCountText.text != $"Kills: {killCount}")
        {
            killCountText.text = $"Kills: {killCount}";
        }
    }
}
