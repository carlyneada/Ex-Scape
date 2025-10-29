using UnityEngine;
using UnityEngine.UI;
public class StaminaUI : MonoBehaviour
{
    Slider slider;              // The actual UI Slider element
    PlayerController player;    // Reference to the player to read stamina from

    void Start()
    {
        // Find the player at runtime; if missing (menus), just disable this UI.
        player = FindObjectOfType<PlayerController>();
        if (!player) { enabled = false; return; }

        // Attach to the main HUD canvas so layering/sorting is consistent.
        var parent = HUD.Ensure().transform;

        // Create the Slider root
        var go = new GameObject("StaminaBar");
        go.transform.SetParent(parent, false);
        slider = go.AddComponent<Slider>();

        // Position + size: top-left corner with a small margin
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);   // top-left anchor
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);   // pivot also top-left
        rt.anchoredPosition = new Vector2(16, -16); // 16px from top-left
        rt.sizeDelta = new Vector2(240, 14);  // width x height

        // Slider range (normalized 0..1)
        slider.minValue = 0f;
        slider.maxValue = 1f;

        // Background track
        var bg = new GameObject("BG").AddComponent<Image>();
        bg.transform.SetParent(go.transform, false);
        bg.color = new Color(0, 0, 0, 0.35f);  // semi-transparent dark
        var bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        slider.targetGraphic = bg; // not animating transitions, but good to set

        // Fill area (inner padded rect so the fill doesn't touch edges)
        var fillArea = new GameObject("FillArea").AddComponent<RectTransform>();
        fillArea.SetParent(go.transform, false);
        fillArea.anchorMin = new Vector2(0f, 0f);
        fillArea.anchorMax = new Vector2(1f, 1f);
        fillArea.offsetMin = new Vector2(2, 2);
        fillArea.offsetMax = new Vector2(-2, -2);

        // Fill image (the green bar that grows)
        var fill = new GameObject("Fill").AddComponent<Image>();
        fill.transform.SetParent(fillArea, false);
        fill.color = new Color(0.4f, 0.9f, 0.4f, 1f);
        var fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = new Vector2(0f, 0f);
        fillRT.anchorMax = new Vector2(1f, 1f);
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;

        // Wire the slider to the fill rect
        slider.fillRect = fillRT;
    }

    void Update()
    {
        // Mirror the player's current stamina (expects PlayerController.GetStamina01()).
        if (player && slider) slider.value = player.GetStamina01();
    }
}