using UnityEngine;

public class PhaseMoveStrategy : IMovementStrategy
{
    private readonly float _speed = 5f;
    private float _timer;

    public void Move(Ghost ghost, Transform target)
    {
        Vector2 dir = ((Vector2)target.position - ghost.Rb.position).normalized;
        ghost.Rb.MovePosition(ghost.Rb.position + dir * _speed * ghost.SpeedMultiplier * Time.fixedDeltaTime);

        _timer += Time.fixedDeltaTime;
        float alpha = 0.3f + 0.5f * Mathf.Abs(Mathf.Sin(_timer * 6f));
        Color c = ghost.Sr.color;
        c.a = alpha;
        ghost.Sr.color = c;
    }
}
