using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
public class SpeedometerUI : MonoBehaviour
{
    private TextMeshProUGUI speedText;
    private Image speedometerBackground;
    private Image speedometerFill;
    
    [Header("Visual Settings")]
    private Color normalSpeedColor = Color.white;
    private Color highSpeedColor = Color.red;
    private float highSpeedThreshold = 8f;
    private float maxDisplaySpeed = 15f;
    
    private PlayerMovement playerMovement;
    private float currentSpeed;
    
    private void Start()
    {
        // Find the player movement component
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        
        if (playerMovement == null)
        {
            Debug.LogError("SpeedometerUI: PlayerMovement component not found!");
            enabled = false;
            return;
        }
        
        // Setup UI positioning in top-left corner
        SetupUI();
    }
    
    private void SetupUI()
    {
        // Get or create Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Create speedometer background panel
        GameObject panelGO = new GameObject("SpeedometerPanel");
        panelGO.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = panelGO.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(20, -20);
        panelRect.sizeDelta = new Vector2(200, 80);
        
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);
        
        // Create speed text
        GameObject textGO = new GameObject("SpeedText");
        textGO.transform.SetParent(panelGO.transform, false);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        speedText = textGO.AddComponent<TextMeshProUGUI>();
        speedText.text = "0 km/h";
        speedText.fontSize = 24;
        speedText.color = normalSpeedColor;
        speedText.alignment = TextAlignmentOptions.Center;
        speedText.fontStyle = FontStyles.Bold;
        
        // Create speed bar background
        GameObject barBGGO = new GameObject("SpeedBarBackground");
        barBGGO.transform.SetParent(panelGO.transform, false);
        
        RectTransform barBGRect = barBGGO.AddComponent<RectTransform>();
        barBGRect.anchorMin = new Vector2(0, 0);
        barBGRect.anchorMax = new Vector2(1, 0);
        barBGRect.pivot = new Vector2(0.5f, 0);
        barBGRect.anchoredPosition = new Vector2(0, 10);
        barBGRect.sizeDelta = new Vector2(-20, 8);
        
        speedometerBackground = barBGGO.AddComponent<Image>();
        speedometerBackground.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Create speed bar fill
        GameObject barFillGO = new GameObject("SpeedBarFill");
        barFillGO.transform.SetParent(barBGGO.transform, false);
        
        RectTransform barFillRect = barFillGO.AddComponent<RectTransform>();
        barFillRect.anchorMin = Vector2.zero;
        barFillRect.anchorMax = Vector2.one;
        barFillRect.offsetMin = Vector2.zero;
        barFillRect.offsetMax = Vector2.zero;
        
        speedometerFill = barFillGO.AddComponent<Image>();
        speedometerFill.color = normalSpeedColor;
        speedometerFill.type = Image.Type.Filled;
        speedometerFill.fillMethod = Image.FillMethod.Horizontal;
    }
    
    private void Update()
    {
        if (playerMovement != null)
        {
            currentSpeed = playerMovement.GetCurrentSpeed();
            UpdateSpeedometer();
        }
    }
    
    private void UpdateSpeedometer()
    {
        // Update speed text
        speedText.text = $"{currentSpeed:F1} km/h";
        
        // Update speed bar fill
        float fillAmount = Mathf.Clamp01(currentSpeed / maxDisplaySpeed);
        speedometerFill.fillAmount = fillAmount;
        
        // Change color based on speed
        Color targetColor = currentSpeed >= highSpeedThreshold ? highSpeedColor : normalSpeedColor;
        speedText.color = targetColor;
        speedometerFill.color = targetColor;
    }
}
#endif