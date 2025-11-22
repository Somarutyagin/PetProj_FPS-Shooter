using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LevelUpView : MonoBehaviour
{
    private const float animOpenTime = 0.5f; //open should be greater than close
    private const float animCloseTime = 0.3f;

    [Inject] private StatsManager statsManager { get; set; }
    [Inject] private GameStateManager gameStateManager { get; set; }
    [Inject] private RarityManager rarityManager { get; set; }

    [SerializeField] private GameObject _levelUpWindow;
    [SerializeField] private Button[] _upgradeButtons = new Button[3];
    [SerializeField] private Material[] _rarityImageBackMaterials = new Material[6];

    private List<Rarity> rarityList = new();

    private UpgradeStatCommand _upgradeCommand;
    private ExperienceManager _expManager;

    private int _pendingLevelUps = 0;
    private CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public void Construct(UpgradeStatCommand upgradeCommand, ExperienceManager expManager)
    {
        _upgradeCommand = upgradeCommand;
        _expManager = expManager;
    }

    private void Start()
    {
        _expManager.OnLevelUp
            .Subscribe(_ =>
            {
                _pendingLevelUps++;
                if (_pendingLevelUps == 1) // Only start if not already showing
                {
                    ShowLevelUpMenuForNext().Forget();
                }
            })
            .AddTo(_disposables);
    }
    private async UniTask ShowLevelUpMenuForNext()
    {
        if (_pendingLevelUps <= 0) return;

        SetupLevelUpWindow();
        await UniTask.Delay((int)(animOpenTime * 1000)); // Wait for setup or animation
        // Wait for the window to be closed (this will be handled in the upgrade selection)
        // The upgrade selection will decrement _pendingLevelUps and call next if needed
    }
    private void SetupLevelUpWindow()
    {
        for (int i = 0; i < 3; i++)
        {
            Rarity rarity = rarityManager.GenerateRandomRarity();

            rarityList.Add(rarity);
            Color rarityColor = rarityManager.GetRarityColor(rarity);
            _upgradeButtons[i].GetComponent<Image>().color = new Color(rarityColor.r / 2, rarityColor.g / 2, rarityColor.b / 2);
            _upgradeButtons[i].transform.GetChild(0).GetComponent<Image>().color = rarityColor;
            _upgradeButtons[i].transform.GetChild(0).GetComponent<Image>().material = _rarityImageBackMaterials[(int)rarity];
            _upgradeButtons[i].transform.GetChild(0).GetChild(1).GetComponent<Text>().text = rarity.ToString();
            _upgradeButtons[i].transform.GetChild(0).GetChild(1).GetComponent<Text>().color = rarityColor;
            var type = i switch
            {
                0 => StatType.Speed,
                1 => StatType.Armor,
                2 => StatType.Lifesteal,
                _ => StatType.Speed,
            };

            float currentValue = statsManager.GetStatPrecent(type).Value;
            float upgradeValue = statsManager.GetCurrentUpgrade(type, rarity);
            _upgradeButtons[i].transform.GetChild(1).GetComponent<Text>().text = $"{type} {currentValue}% + {upgradeValue - currentValue}% -> {upgradeValue}%";
        }
        gameStateManager.ChangeState(new PausedState(gameStateManager));

        _levelUpWindow.SetActive(true);
        _levelUpWindow.transform.localScale = Vector3.zero; // Start from scale 0
        _levelUpWindow.transform.DOScale(Vector3.one, animOpenTime).SetEase(Ease.OutBack).SetUpdate(true); // Animate scale to 1 with DOTween
        // Assuming _upgradeButtons are set up, add listeners for selection
        for (int i = 0; i < _upgradeButtons.Length; i++)
        {
            int index = i; // Capture for lambda
            _upgradeButtons[i].onClick.RemoveAllListeners(); // Clear previous listeners
            _upgradeButtons[i].onClick.AddListener(() => OnUpgradeSelected(index));
        }
    }
    private async void OnUpgradeSelected(int index)
    {
        // Apply the upgrade based on index
        Rarity selectedRarity = rarityList[index];
        StatType selectedType = index switch
        {
            0 => StatType.Speed,
            1 => StatType.Armor,
            2 => StatType.Lifesteal,
            _ => StatType.Speed,
        };
        _upgradeCommand.Execute(selectedType, selectedRarity);
        // Animate closing the window
        await _levelUpWindow.transform.DOScale(Vector3.zero, animCloseTime).SetEase(Ease.InBack).SetUpdate(true).AsyncWaitForCompletion();
        _levelUpWindow.SetActive(false);
        // Resume game state
        gameStateManager.ChangeState(new PlayingState(gameStateManager)); // Assuming there's a playing state
        // Decrement pending and show next if any
        _pendingLevelUps--;
        if (_pendingLevelUps > 0)
        {
            await UniTask.Delay((int)((animOpenTime - animCloseTime) * 1000)); // Small delay between windows
            ShowLevelUpMenuForNext().Forget();
        }
    }
    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}