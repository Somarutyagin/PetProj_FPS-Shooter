using UnityEngine;
using UnityEngine.UI;

public class KillCounter : MonoBehaviour
{
    [SerializeField] private Text killCountText;

    private int killCount = 0;
    private EnemyAI[] enemies;

    private void Awake()
    {
        if (killCountText != null) killCountText.text = "Kills: 0";
    }
    public void UpdateKillUI()
    {
        killCount++;
        if (killCountText != null && killCountText.text != $"Kills: {killCount}")
        {
            killCountText.text = $"Kills: {killCount}";
        }
    }
}
