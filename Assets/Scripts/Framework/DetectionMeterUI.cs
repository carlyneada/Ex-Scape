// Simple overlay that shows the player's awkwardness (0..1) as a slider + percent.

using UnityEngine;
using UnityEngine.UI;

public class DetectionMeterUI : MonoBehaviour
{
    Slider _slider;
    Text _label;
    Text _percent;
    DetectionMeter _meter;

    void Awake()
    {
        BuildUI();
    }

    void Start()
    {
        // Find the player's meter once at start (will re-find if missing).
        var player = FindObjectOfType<PlayerController>();
        if (player) _meter = player.GetComponent<DetectionMeter>();
    }

    void Update()
    {
        if (_meter == null)
        {
            var player = FindObjectOfType<PlayerController>();
            if (player) _meter = player.GetComponent<DetectionMeter>();
        }

        float v = (_meter != null) ? _meter.current01 : 0f;
        if (_slider)  _slider.value = v;
        if (_percent) _percent.text = $"{Mathf.RoundToInt(v * 100f)}%";
    }

    void BuildUI()
    {
        // Canvas (Screen Space Overlay)
        var canvasGO = new GameObject("AwkCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvasGO);

        // Panel holder
        var panel = new GameObject("AwkPanel");
        panel.transform.SetParent(canvasGO.transform, false);
        var prt = panel.AddComponent<RectTransform>();
        prt.anchorMin = new Vector2(0.5f, 1f);
        prt.anchorMax = new Vector2(0.5f, 1f);
        prt.pivot     = new Vector2(0.5f, 1f);
        prt.anchoredPosition = new Vector2(0, -16); // top center
        prt.sizeDelta = new Vector2(320, 28);

        // Label
        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(panel.transform, false);
        _label = labelGO.AddComponent<Text>();
        _label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _label.fontSize = 16;
        _label.alignment = TextAnchor.MiddleLeft;
        _label.text = "Awkwardness";
        var lrt = _label.GetComponent<RectTransform>();
        lrt.anchorMin = new Vector2(0, 0);
        lrt.anchorMax = new Vector2(0, 1);
        lrt.sizeDelta = new Vector2(120, 0);
        lrt.anchoredPosition = new Vector2(8, 0);

        // Slider root
        var sliderRoot = new GameObject("Slider");
        sliderRoot.transform.SetParent(panel.transform, false);
        var srt = sliderRoot.AddComponent<RectTransform>();
        srt.anchorMin = new Vector2(0, 0);
        srt.anchorMax = new Vector2(1, 1);
        srt.offsetMin = new Vector2(120, 4);
        srt.offsetMax = new Vector2(60, -4);

        // Slider component
        _slider = sliderRoot.AddComponent<Slider>();
        _slider.minValue = 0f;
        _slider.maxValue = 1f;
        _slider.value = 0f;
        _slider.transition = Selectable.Transition.None;

        // Background
        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(sliderRoot.transform, false);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0.35f);
        var bgRT = bgImg.GetComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0, 0);
        bgRT.anchorMax = new Vector2(1, 1);
        bgRT.offsetMin = bgRT.offsetMax = Vector2.zero;

        // Fill Area
        var fillAreaGO = new GameObject("Fill Area");
        fillAreaGO.transform.SetParent(sliderRoot.transform, false);
        var faRT = fillAreaGO.AddComponent<RectTransform>();
        faRT.anchorMin = new Vector2(0, 0.25f);
        faRT.anchorMax = new Vector2(1, 0.75f);
        faRT.offsetMin = new Vector2(2, 0);
        faRT.offsetMax = new Vector2(-2, 0);

        // Fill
        var fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(fillAreaGO.transform, false);
        var fillImg = fillGO.AddComponent<Image>();
        fillImg.color = new Color(1f, 0.65f, 0.2f, 0.95f); // orange bar
        var fillRT = fillImg.GetComponent<RectTransform>();
        fillRT.anchorMin = new Vector2(0, 0);
        fillRT.anchorMax = new Vector2(1, 1);
        fillRT.offsetMin = fillRT.offsetMax = Vector2.zero;

        _slider.fillRect = fillRT;
        _slider.targetGraphic = fillImg;
        _slider.direction = Slider.Direction.LeftToRight;

        // Percent text (right)
        var pctGO = new GameObject("Percent");
        pctGO.transform.SetParent(panel.transform, false);
        _percent = pctGO.AddComponent<Text>();
        _percent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _percent.fontSize = 16;
        _percent.alignment = TextAnchor.MiddleRight;
        _percent.text = "0%";
        var pr = _percent.GetComponent<RectTransform>();
        pr.anchorMin = new Vector2(1, 0);
        pr.anchorMax = new Vector2(1, 1);
        pr.sizeDelta = new Vector2(52, 0);
        pr.anchoredPosition = new Vector2(-6, 0);
    }
}
