using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour, IHealth
{
    [Header("HP Bar Settings")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image hpBarDelta;
    [SerializeField] private Text hpBarText;
    [SerializeField] private Material normalEnemyMat;
    [SerializeField] private Material takingDamageEnemyMat;
    public readonly float MaxHealth = 100f;
    public event System.Action OnDeath;
    private const float bodyAnimationDuration = 0.1f;
    private float currentHealth;

    private const int _iterationCount = 30;
    private const float _animTime = 0.2f;
    private float previousHealth;
    private float _timerH = 0f;
    private int _currentIterationH = 0;
    private bool _isAnimatingH = false;
    private float _targetFillH;
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

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            UpdateHPBar(previousHealth);
            StartCoroutine(AnimateBody());
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

        if (hpBarText != null)
        {
            hpBarText.text = string.Format("{0:F0}", currentHealth);
        }

        if (this.previousHealth < currentHealth)
        {
            hpBarDelta.fillAmount = (float)currentHealth / MaxHealth;
            StartAmmoBarDelta(true);
        }
        else
        {
            hpBar.fillAmount = (float)currentHealth / MaxHealth;
            StartAmmoBarDelta(false);
        }
    }
    private void StartAmmoBarDelta(bool less)
    {
        _isAnimatingH = true;
        _currentIterationH = 0;
        _timerH = 0f;

        if (less)
        {
            _targetFillH = hpBarDelta.fillAmount - hpBar.fillAmount;
        }
        else
        {
            _targetFillH = hpBar.fillAmount - hpBarDelta.fillAmount;
        }
    }

    private void FixedUpdate()
    {
        if (_isAnimatingH)
        {
            _timerH += Time.fixedDeltaTime;
            if (_currentIterationH < _iterationCount)
            {
                if (_timerH >= (_animTime / _iterationCount))
                {
                    _timerH -= (_animTime / _iterationCount);
                    _currentIterationH++;

                    if (previousHealth < currentHealth)
                    {
                        hpBar.fillAmount += _targetFillH / _iterationCount;
                    }
                    else
                    {
                        hpBarDelta.fillAmount += _targetFillH / _iterationCount;
                    }
                }
            }
            else
            {
                _isAnimatingH = false;
                previousHealth = currentHealth;
            }
        }
    }
    private IEnumerator AnimateBody()
    {
        GetComponent<MeshRenderer>().material = takingDamageEnemyMat;

        yield return new WaitForSeconds(bodyAnimationDuration);

        GetComponent<MeshRenderer>().material = normalEnemyMat;
    }
}
