using UnityEngine;

public class NormalMoveStrategy : IPlayerMoveStrategy
{
    public void Move(Rigidbody2D rb, float speed)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        rb.MovePosition(rb.position + new Vector2(h, v).normalized * speed * Time.fixedDeltaTime);
    }
}
