using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour, IHealth
{
    
    [Header("HP Bar Settings")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image hpBarDelta;
    public readonly float MaxHealth = 100f;
    public event System.Action OnDeath;
    private const float deltaAnimationDuration = 0.3f;
    private float currentHealth;
    
    private Coroutine deltaAnimationCoroutine;
    private void Awake()
    {
        SetHealth(MaxHealth);
        InitializeHPBar();
    }
    
    private void InitializeHPBar()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = 1f;
        }
        if (hpBarDelta != null)
        {
            hpBarDelta.fillAmount = 1f;
        }
    }
    public void SetHealth(float health)
    {
        float previousHealth = currentHealth;
        currentHealth = health;
        UpdateHPBar(previousHealth);
    }
    public void TakeDamage(float damage)
    {
        float previousHealth = currentHealth;
        currentHealth -= damage;
        UpdateHPBar(previousHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        OnDeath?.Invoke();

        gameObject.SetActive(false);

        if (CompareTag("Player"))
        {
            Invoke(nameof(RestartScene), 2f);
        }
    }
    private void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public bool IsDead() => currentHealth <= 0;
    
    private void UpdateHPBar(float previousHealth)
    {
        if (hpBar == null || hpBarDelta == null) return;
        
        // Calculate health percentages
        float previousHealthPercent = previousHealth / MaxHealth;
        float currentHealthPercent = currentHealth / MaxHealth;
        
        // Immediately update the main HP bar
        hpBar.fillAmount = currentHealthPercent;
        
        // Start delta animation if there's a change
        if (Mathf.Abs(previousHealthPercent - currentHealthPercent) > 0.001f)
        {
            if (deltaAnimationCoroutine != null)
            {
                StopCoroutine(deltaAnimationCoroutine);
            }
            deltaAnimationCoroutine = StartCoroutine(AnimateDeltaBar(previousHealthPercent, currentHealthPercent));
        }
    }
    
    private IEnumerator AnimateDeltaBar(float fromPercent, float toPercent)
    {
        // Set delta bar to previous health level
        hpBarDelta.fillAmount = fromPercent;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < deltaAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / deltaAnimationDuration;
            
            // Smooth interpolation
            float currentDeltaPercent = Mathf.Lerp(fromPercent, toPercent, progress);
            hpBarDelta.fillAmount = currentDeltaPercent;
            
            yield return null;
        }
        
        // Ensure final value is set
        hpBarDelta.fillAmount = toPercent;
        deltaAnimationCoroutine = null;
    }
}
