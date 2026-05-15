using UnityEngine;

public interface IMovementStrategy
{
    void Move(Ghost ghost, Transform target);
}
