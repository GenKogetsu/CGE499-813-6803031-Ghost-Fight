using UnityEngine;

public abstract class WeaponDecorator : IWeapon
{
    protected readonly IWeapon _wrapped;
    protected WeaponDecorator(IWeapon wrapped) => _wrapped = wrapped;

    public virtual void Tick(Vector2 from, Vector2 dir, BulletConfig cfg, bool firePressed)
        => _wrapped.Tick(from, dir, cfg, firePressed);
}
