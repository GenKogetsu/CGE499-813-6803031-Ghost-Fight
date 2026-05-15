using UnityEngine;

public class HeavyGun : IWeapon
{
    private readonly GameObject _prefab;
    private readonly Transform  _firePoint;
    private float _cooldown;

    public HeavyGun(GameObject prefab, Transform firePoint)
    {
        _prefab    = prefab;
        _firePoint = firePoint;
    }

    public void Tick(Vector2 from, Vector2 dir, BulletConfig cfg, bool firePressed)
    {
        _cooldown -= Time.deltaTime;
        if (!firePressed || _cooldown > 0f) return;

        _cooldown   = 0.9f;
        cfg.damage *= 4f;
        cfg.scale  *= 5f;
        cfg.speed  *= 0.55f;

        if (_prefab == null || _firePoint == null) return;
        Object.Instantiate(_prefab, _firePoint.position, Quaternion.identity)
              .GetComponent<Bullet>()?.Init(dir, cfg);
    }
}
