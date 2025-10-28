// Assets/Scripts/Enemies/EnemyController.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyController : MonoBehaviour
{
    [Header("Refs")]
    public Transform[] waypoints;     // expected size 2 (ping-pong)
    public FieldOfView2D fov;

    [Header("Movement")]
    public float patrolSpeed = 2.8f;  // a touch faster than before
    public float chaseSpeed  = 4.8f;  // noticeably quicker to close distance
    public float arriveDist  = 0.05f;

    [Header("Chase Memory")]
    public float memorySeconds = 1.0f; // keep chasing briefly after losing sight

    int _wpIndex = 0;
    int _wpDir = +1;                  // ping-pong between 0 and 1
    Rigidbody2D _rb;
    SpriteRenderer _sr;

    public bool IsChasing { get; private set; }
    float _memoryTimer;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        _sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        var player = FindObjectOfType<PlayerController>();
        if (!player) return;

        // Determine chasing state — now FOV is more responsive
        bool sees = fov ? fov.CanSee(player) : false;
        if (sees)
        {
            IsChasing = true;
            _memoryTimer = memorySeconds;
        }
        else if (IsChasing)
        {
            _memoryTimer -= Time.deltaTime;
            if (_memoryTimer <= 0f) IsChasing = false;
        }

        // Decide target and speed
        Vector2 target;
        float speed;
        if (IsChasing)
        {
            target = player.transform.position;
            speed = chaseSpeed;
        }
        else
        {
            // Patrol ping-pong between waypoints
            if (waypoints == null || waypoints.Length < 2)
            {
                _rb.velocity = Vector2.zero;
                return;
            }

            target = waypoints[_wpIndex].position;
            speed = patrolSpeed;

            if (Vector2.Distance(transform.position, target) <= arriveDist)
            {
                if (_wpIndex == 0) _wpDir = +1;
                else if (_wpIndex == waypoints.Length - 1) _wpDir = -1;
                _wpIndex = Mathf.Clamp(_wpIndex + _wpDir, 0, waypoints.Length - 1);
            }
        }

        // Move
        Vector2 pos = transform.position;
        Vector2 dir = (target - pos).normalized;
        _rb.velocity = dir * speed;

        // Face velocity (no spin)
        if (_rb.velocity.sqrMagnitude > 0.001f)
        {
            float sx = Mathf.Sign(_rb.velocity.x);
            if (sx == 0f) sx = 1f;
            transform.localScale = new Vector3(sx, 1f, 1f);
        }
        transform.rotation = Quaternion.identity;
    }

    public void Freeze()
    {
        if (_rb) _rb.velocity = Vector2.zero;
        enabled = false;
    }

    /// Visibility contribution for DetectionMeter (0..1+).
    public float VisibilityOfPlayer()
    {
        var player = FindObjectOfType<PlayerController>();
        if (!player || fov == null) return 0f;

        float v = fov.VisibilityStrength(player);
        if (IsChasing) v = Mathf.Max(v, 0.7f); // while chasing, at least a strong visibility
        return Mathf.Clamp01(v);
    }
}
