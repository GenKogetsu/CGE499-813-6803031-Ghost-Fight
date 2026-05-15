using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ItemPickup : MonoBehaviour
{
    private IItemVisitor _visitor;

    public void Init(IItemVisitor visitor, Color color)
    {
        _visitor = visitor;
        GetComponent<SpriteRenderer>().color = color;
    }

    void Awake()
    {
        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = 0.5f;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        col.GetComponent<PlayerController>()?.Accept(_visitor);
        Destroy(gameObject);
    }
}
