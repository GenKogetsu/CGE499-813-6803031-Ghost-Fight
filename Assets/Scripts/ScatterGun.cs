using UnityEngine;

public class ScatterGun : IWeapon
{
    private readonly GameObject _prefab;
    private readonly Transform  _firePoint;
    private float _cooldown;

    public ScatterGun(GameObject prefab, Transform firePoint)
    {
        _prefab    = prefab;
        _firePoint = firePoint;
    }

    public void Tick(Vector2 from, Vector2 dir, BulletConfig cfg, bool firePressed)
    {
        _cooldown -= Time.deltaTime;
        if (!firePressed || _cooldown > 0f) return;

        _cooldown   = 0.35f;
        cfg.damage *= 0.6f;
        cfg.scale  *= 3f;

        for (int i = -2; i <= 2; i++)
            Spawn(Rotate(dir, i * 14f), cfg);
    }

    void Spawn(Vector2 dir, BulletConfig cfg)
    {
        if (_prefab == null || _firePoint == null) return;
        Object.Instantiate(_prefab, _firePoint.position, Quaternion.identity)
              .GetComponent<Bullet>()?.Init(dir, cfg);
    }

    static Vector2 Rotate(Vector2 v, float deg)
    {
        float r = deg * Mathf.Deg2Rad;
        float c = Mathf.Cos(r), s = Mathf.Sin(r);
        return new Vector2(c * v.x - s * v.y, s * v.x + c * v.y);
    }
}
