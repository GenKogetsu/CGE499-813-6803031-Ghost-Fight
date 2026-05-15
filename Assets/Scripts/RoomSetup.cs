using UnityEngine;

[ExecuteAlways]
public class RoomSetup : MonoBehaviour
{
    [SerializeField] public float halfW = 8.5f;
    [SerializeField] public float halfH = 5f;

    public float HalfW => halfW;
    public float HalfH => halfH;

    void Awake()
    {
        Make("Wall_Top",    new Vector2(0,       halfH),  new Vector2(halfW * 2 + 1f, 0.5f));
        Make("Wall_Bottom", new Vector2(0,      -halfH),  new Vector2(halfW * 2 + 1f, 0.5f));
        Make("Wall_Left",   new Vector2(-halfW,  0),      new Vector2(0.5f, halfH * 2));
        Make("Wall_Right",  new Vector2( halfW,  0),      new Vector2(0.5f, halfH * 2));
    }

    static void Make(string name, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.tag = "Wall";
        go.transform.position = pos;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);

        var sr  = go.AddComponent<SpriteRenderer>();
        sr.sprite = WhiteSquare();
        sr.color  = new Color(0.25f, 0.25f, 0.3f);
        sr.sortingOrder = -1;

        go.AddComponent<BoxCollider2D>();
    }

    static Sprite WhiteSquare()
    {
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
