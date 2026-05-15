using UnityEngine;

public interface IWeapon
{
    void Tick(Vector2 from, Vector2 dir, BulletConfig cfg, bool firePressed);
}
