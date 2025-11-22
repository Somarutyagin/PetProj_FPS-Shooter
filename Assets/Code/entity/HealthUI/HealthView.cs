using Cysharp.Threading.Tasks;
using DG.Tweening;
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

    private float previousHealth;

    private void Start()
    {
        InitializeHPBar();
    }

    public void Initialize()
    {
        ViewModel.Model.CurrentHealth.Subscribe(health =>
        {
            float fill = (float)health / ViewModel.Model.MaxHealth;

            if (previousHealth >= health)
            {
                hpBar.fillAmount = fill;
                hpBarDelta.DOFillAmount(fill, 0.2f); // Animate fill amount with DOTween
            }
            else
            {
                hpBarDelta.fillAmount = fill;
                hpBar.DOFillAmount(fill, 0.2f); // Animate fill amount with DOTween
            }
            previousHealth = health;
            if (hpBarText != null)
            {
                hpBarText.text = $"{(int)health}/{(int)ViewModel.Model.MaxHealth}";
            }
        })
        .AddTo(this);
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