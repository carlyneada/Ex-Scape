// Assets/Scripts/Interactables/HideSpot.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class HideSpot : MonoBehaviour
{
    PlayerController _inside;
    Canvas _promptCanvas;
    Text _promptText;

    void Reset()
    {
        var bc = GetComponent<BoxCollider2D>();
        bc.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var pc = other.GetComponent<PlayerController>();
        if (!pc) return;
        _inside = pc;
        ShowPrompt(pc, pc.IsHidden() ? "E: Unhide" : "E: Hide");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var pc = other.GetComponent<PlayerController>();
        if (!pc || _inside != pc) return;

        // Auto-unhide if they walk out while hidden
        if (pc.IsHidden()) pc.SetHidden(false);

        _inside = null;
        HidePrompt();
    }

    void Update()
    {
        if (_inside == null) return;

        // Keep prompt near the player
        if (_promptCanvas)
        {
            _promptCanvas.transform.position = _inside.transform.position + new Vector3(0, 1.2f, 0);
        }

        // Require pressing E to toggle hide/unhide
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool newHidden = !_inside.IsHidden();
            _inside.SetHidden(newHidden); // plays hide sound on entering hidden
            UpdatePrompt(newHidden ? "E: Unhide" : "E: Hide");
        }
    }

    // ------- prompt helpers -------

    void ShowPrompt(PlayerController pc, string text)
    {
        if (_promptCanvas != null) return;

        _promptCanvas = new GameObject("HidePromptCanvas").AddComponent<Canvas>();
        _promptCanvas.renderMode = RenderMode.WorldSpace;
        _promptCanvas.sortingOrder = 25;
        var rt = _promptCanvas.GetComponent<RectTransform>();
        rt.localScale = Vector3.one * 0.01f;
        rt.sizeDelta = new Vector2(140, 50);
        _promptCanvas.transform.position = pc.transform.position + new Vector3(0, 1.2f, 0);

        _promptText = new GameObject("Text").AddComponent<Text>();
        _promptText.transform.SetParent(_promptCanvas.transform, false);
        _promptText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _promptText.fontSize = 48;
        _promptText.alignment = TextAnchor.MiddleCenter;
        _promptText.color = new Color(0.9f, 0.95f, 1f, 0.95f);

        var tr = _promptText.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;

        _promptText.text = text;
    }

    void UpdatePrompt(string text)
    {
        if (_promptText) _promptText.text = text;
    }

    void HidePrompt()
    {
        if (_promptCanvas) Destroy(_promptCanvas.gameObject);
        _promptCanvas = null;
        _promptText = null;
    }
}
