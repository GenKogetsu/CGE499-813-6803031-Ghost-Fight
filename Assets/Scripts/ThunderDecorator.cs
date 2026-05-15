using UnityEngine;

public class ThunderDecorator : WeaponDecorator
{
    public ThunderDecorator(IWeapon wrapped) : base(wrapped) { }

    public override void Tick(Vector2 from, Vector2 dir, BulletConfig cfg, bool firePressed)
    {
        cfg.damage += 1f;
        cfg.scale  *= 1.8f;
        cfg.color   = new Color(0.85f, 1f, 0.1f);
        base.Tick(from, dir, cfg, firePressed);
    }
}
