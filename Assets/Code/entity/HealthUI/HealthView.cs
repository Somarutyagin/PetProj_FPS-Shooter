using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;
    [Header("HP Bar Settings")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image hpBarDelta;
    [SerializeField] private Text hpBarText;
    [SerializeField] private Material normalEnemyMat;
    [SerializeField] private Material takingDamageEnemyMat;

    public IHealthViewModel ViewModel { get; set; }

    private const float BodyAnimationDuration = 0.1f;
    private const int IterationCount = 30;
    private const float AnimTime = 0.2f;

    private void Start()
    {
        InitializeHPBar();
    }

    public void Initialize()
    {
        ViewModel.Model.CurrentHealth.Subscribe(UpdateHPBar).AddTo(this);
        ViewModel.OnDeath.Subscribe(_ => Die()).AddTo(this);
        ViewModel.TakeDamageCommand.Subscribe(_ => AnimateBody().Forget()).AddTo(this);
    }
    private void OnDestroy()
    {
        ViewModel?.Dispose();
    }

    private void InitializeHPBar()
    {
        if (hpBar != null) hpBar.fillAmount = 1f;
        if (hpBarDelta != null) hpBarDelta.fillAmount = 1f;
    }

    private void UpdateHPBar(float currentHealth)
    {
        if (hpBar == null || hpBarDelta == null) return;

        if (hpBarText != null)
        {
            hpBarText.text = $"{currentHealth:F0}";
        }

        float fillAmount = currentHealth / ViewModel.Model.MaxHealth;
        hpBar.fillAmount = fillAmount;
        AnimateHPBarDelta(fillAmount).Forget();
    }

    private async UniTask AnimateHPBarDelta(float targetFill)
    {
        float startFill = hpBarDelta.fillAmount;
        float delta = targetFill - startFill;
        for (int i = 0; i < IterationCount; i++)
        {
            await UniTask.Delay((int)(AnimTime / IterationCount * 1000), DelayType.DeltaTime);
            hpBarDelta.fillAmount += delta / IterationCount;
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);

        if (enemyAI != null)
        {
            enemyAI.OnEnemyDeathHandler();
        }
    }

    private async UniTask AnimateBody()
    {
        var renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = takingDamageEnemyMat;
            await UniTask.Delay((int)(BodyAnimationDuration * 1000));
            renderer.material = normalEnemyMat;
        }
    }
}