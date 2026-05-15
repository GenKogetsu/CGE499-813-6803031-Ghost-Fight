using UnityEngine;

public class SolidMoveStrategy : IMovementStrategy
{
    private readonly float _speed = 3.5f;

    public void Move(Ghost ghost, Transform target)
    {
        Vector2 dir = ((Vector2)target.position - ghost.Rb.position).normalized;
        ghost.Rb.MovePosition(ghost.Rb.position + dir * _speed * ghost.SpeedMultiplier * Time.fixedDeltaTime);
    }
}
