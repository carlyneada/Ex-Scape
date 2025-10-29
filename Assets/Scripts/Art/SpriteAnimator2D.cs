using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator2D : MonoBehaviour
{
    public Sprite[] idleRunFrames;
    public float framesPerSecond = 8f;

    SpriteRenderer sr;
    float t;

    void Awake() { sr = GetComponent<SpriteRenderer>(); }

    void Update()
    {
        if (idleRunFrames == null || idleRunFrames.Length == 0) return;
        t += Time.deltaTime * framesPerSecond;
        int idx = Mathf.FloorToInt(t) % idleRunFrames.Length;
        sr.sprite = idleRunFrames[idx];
    }
}
