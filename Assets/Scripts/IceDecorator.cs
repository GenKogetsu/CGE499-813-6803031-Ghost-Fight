using UnityEngine;

public class IceDecorator : WeaponDecorator
{
    public IceDecorator(IWeapon wrapped) : base(wrapped) { }

    public override void Tick(Vector2 from, Vector2 dir, BulletConfig cfg, bool firePressed)
    {
        cfg.applySlowOnHit = true;
        cfg.slowFactor     = 0.35f;
        cfg.slowDuration   = 2.5f;
        cfg.color          = new Color(0.3f, 0.75f, 1f);
        base.Tick(from, dir, cfg, firePressed);
    }
}
