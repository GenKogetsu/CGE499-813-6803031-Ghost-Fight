using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Bullet : MonoBehaviour
{
    private float      _speed  = 12f;
    private float      _damage = 1f;
    private Vector2    _dir;
    private BulletConfig _cfg;

    void Awake()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType     = RigidbodyType2D.Kinematic;

        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = 0.5f;

        Destroy(gameObject, 3f);
    }

    public void Init(Vector2 direction, BulletConfig cfg)
    {
        _dir    = direction.normalized;
        _cfg    = cfg;
        _speed  = cfg.speed;
        _damage = cfg.damage;
        transform.localScale = Vector3.one * cfg.scale;
        GetComponent<SpriteRenderer>().color = cfg.color;
    }

    void Update() => transform.Translate(_dir * _speed * Time.deltaTime, Space.World);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ghost"))
        {
            var ghost = other.GetComponent<Ghost>();
            ghost?.TakeDamage(Mathf.RoundToInt(_damage));
            ghost?.HitFlash(_cfg.color);
            if (_cfg.applySlowOnHit) ghost?.ApplySlow(_cfg.slowFactor, _cfg.slowDuration);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
