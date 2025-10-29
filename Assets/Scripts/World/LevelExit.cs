using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelExit : MonoBehaviour
{
    bool consumed;

    void Awake()
    {
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed) return;
        if (!other.GetComponent<PlayerController>()) return;

        consumed = true;
        GameManager.Instance.Win();
    }
}
