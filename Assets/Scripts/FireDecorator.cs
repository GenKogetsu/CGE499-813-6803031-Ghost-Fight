using UnityEngine;

public class FireDecorator : WeaponDecorator
{
    public FireDecorator(IWeapon wrapped) : base(wrapped) { }

    public override void Tick(Vector2 from, Vector2 dir, BulletConfig cfg, bool firePressed)
    {
        cfg.damage += 2f;
        cfg.color   = new Color(1f, 0.35f, 0.05f);
        base.Tick(from, dir, cfg, firePressed);
    }
}
