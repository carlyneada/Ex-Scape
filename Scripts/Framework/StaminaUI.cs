using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    Slider slider;
    PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        if (!player) { enabled = false; return; }

        var parent = HUD.Ensure().transform;

        var go = new GameObject("StaminaBar");
        go.transform.SetParent(parent, false);
        slider = go.AddComponent<Slider>();

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot     = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(16, -16);
        rt.sizeDelta = new Vector2(240, 14);

        slider.minValue = 0f;
        slider.maxValue = 1f;

        var bg = new GameObject("BG").AddComponent<Image>();
        bg.transform.SetParent(go.transform, false);
        bg.color = new Color(0,0,0,0.35f);
        var bgRT = bg.GetComponent<RectTransform>(); bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one; bgRT.offsetMin = Vector2.zero; bgRT.offsetMax = Vector2.zero;
        slider.targetGraphic = bg;

        var fillArea = new GameObject("FillArea").AddComponent<RectTransform>();
        fillArea.SetParent(go.transform, false);
        fillArea.anchorMin = new Vector2(0f,0f);
        fillArea.anchorMax = new Vector2(1f,1f);
        fillArea.offsetMin = new Vector2(2,2);
        fillArea.offsetMax = new Vector2(-2,-2);

        var fill = new GameObject("Fill").AddComponent<Image>();
        fill.transform.SetParent(fillArea, false);
        fill.color = new Color(0.4f,0.9f,0.4f,1f);
        var fillRT = fill.GetComponent<RectTransform>(); fillRT.anchorMin = new Vector2(0f,0f); fillRT.anchorMax = new Vector2(1f,1f); fillRT.offsetMin = Vector2.zero; fillRT.offsetMax = Vector2.zero;

        slider.fillRect = fillRT;
    }

    void Update()
    {
        if (player && slider) slider.value = player.GetStamina01();
    }
}
