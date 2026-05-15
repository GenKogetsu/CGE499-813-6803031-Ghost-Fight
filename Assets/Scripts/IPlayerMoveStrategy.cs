using UnityEngine;

public interface IPlayerMoveStrategy
{
    void Move(Rigidbody2D rb, float speed);
}
