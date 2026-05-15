using UnityEngine;

public class LaserGun : IWeapon
{
    private readonly Transform    _firePoint;
    private readonly LineRenderer _lr;
    private readonly float        _chargeRate;

    private float   _charge;
    private bool    _wasPressed;
    private float   _flashTimer;
    private Vector2 _flashFrom;
    private Vector2 _flashEnd;
    private Color   _flashColor;
    private float   _flashWidth;

    public float ChargePercent => _charge;

    public LaserGun(Transform firePoint, LineRenderer lr, float chargeRate = 0.8f)
    {
        _firePoint  = firePoint;
        _lr         = lr;
        _chargeRate = chargeRate;
    }

    public void Tick(Vector2 from, Vector2 dir, BulletConfig cfg, bool firePressed)
    {
        if (_lr == null) return;

        if (_flashTimer > 0f)
        {
            _flashTimer -= Time.deltaTime;
            float alpha = _flashTimer / 0.12f;
            Color c = _flashColor; c.a = alpha;
            _lr.startColor = _lr.endColor = c;
            _lr.startWidth = _lr.endWidth = _flashWidth;
            _lr.enabled = true;
            _lr.SetPosition(0, _flashFrom);
            _lr.SetPosition(1, _flashEnd);
            return;
        }

        if (firePressed)
        {
            _charge = Mathf.MoveTowards(_charge, 1f, _chargeRate * Time.deltaTime);
            _wasPressed = true;

            float previewWidth = Mathf.Lerp(0.02f, 0.08f, _charge);
            Color previewColor = Color.Lerp(cfg.color, Color.white, _charge * 0.6f);
            previewColor.a = Mathf.Lerp(0.3f, 0.9f, _charge);

            _lr.startColor = _lr.endColor = previewColor;
            _lr.startWidth = _lr.endWidth = previewWidth;
            _lr.enabled = true;
            _lr.SetPosition(0, from);
            _lr.SetPosition(1, GetWallEndPoint(from, dir));
        }
        else if (_wasPressed && _charge > 0.05f)
        {
            Fire(from, dir, cfg);
            _wasPressed = false;
        }
        else
        {
            _charge     = 0f;
            _wasPressed = false;
            _lr.enabled = false;
        }
    }

    void Fire(Vector2 from, Vector2 dir, BulletConfig cfg)
    {
        float damageScale = (_charge >= 1f) ? 6f : 1f + Mathf.Floor(_charge / 0.4f);
        int   damage      = Mathf.RoundToInt(cfg.damage * damageScale * 2.5f);

        RaycastHit2D[] hits = Physics2D.RaycastAll(from, dir, 30f);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        Vector2 endPt = from + dir * 30f;

        foreach (var hit in hits)
        {
            if (hit.collider == null)              continue;
            if (hit.collider.CompareTag("Player")) continue;

            if (hit.collider.CompareTag("Ghost"))
            {
                var ghost = hit.collider.GetComponent<Ghost>();
                ghost?.TakeDamage(damage);
                ghost?.HitFlash(cfg.color);
                if (cfg.applySlowOnHit) ghost?.ApplySlow(cfg.slowFactor, cfg.slowDuration);
                continue;
            }

            if (hit.collider.CompareTag("Wall")) { endPt = hit.point; break; }
        }

        float w = Mathf.Lerp(0.06f, 0.28f, _charge);
        Color c = Color.Lerp(cfg.color, Color.white, _charge * 0.7f);
        _flashFrom  = from;
        _flashEnd   = endPt;
        _flashColor = c;
        _flashWidth = w;
        _flashTimer = 0.12f;

        _charge = 0f;
    }

    Vector2 GetWallEndPoint(Vector2 from, Vector2 dir)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(from, dir, 30f);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (var hit in hits)
        {
            if (hit.collider == null)              continue;
            if (hit.collider.CompareTag("Player")) continue;
            if (hit.collider.CompareTag("Wall"))   return hit.point;
        }
        return from + dir * 30f;
    }
}
