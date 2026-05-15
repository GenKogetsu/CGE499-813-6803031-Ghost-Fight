using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Ghost : MonoBehaviour
{
    public Rigidbody2D    Rb              { get; private set; }
    public SpriteRenderer Sr              { get; private set; }
    public float          SpeedMultiplier { get; private set; } = 1f;

    [SerializeField] private int   maxHp      = 4;
    [SerializeField] private float dropChance = 0.4f;

    private IMovementStrategy _strategy;
    private Transform _player;
    private int   _hp;
    private bool  _isPhase;
    private float _slowTimer;
    private Color _baseColor;
    private float _hitFlashTimer;
    private Color _hitFlashColor;

    private static readonly Color[] _itemColors =
    {
        new Color(0.2f, 0.5f, 1f),
        new Color(0.2f, 1f,   0.3f),
        new Color(1f,   0.7f, 0.1f),
    };

    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Sr = GetComponent<SpriteRenderer>();
        Rb.gravityScale   = 0f;
        Rb.freezeRotation = true;
    }

    void Start()
    {
        _player = GameObject.FindWithTag("Player")?.transform;
        _hp     = maxHp;
        ApplySolidSetup();
    }

    public void SetStrategy(IMovementStrategy strategy) => _strategy = strategy;

    void FixedUpdate()
    {
        if (_player == null) return;
        _strategy?.Move(this, _player);
    }

    public void TakeDamage(int dmg)
    {
        _hp -= dmg;

        if (_hp <= 0)
        {
            GameFacade.Instance?.AddScore(10);
            DropItem();
            Destroy(gameObject);
            return;
        }

        if (!_isPhase && _hp <= maxHp / 2)
            ApplyPhaseSetup();
    }

    public void ApplySlow(float factor, float duration)
    {
        SpeedMultiplier = factor;
        _slowTimer      = duration;
        Color c = Sr.color; c.b = 1f; Sr.color = c;
    }

    public void HitFlash(Color elementColor)
    {
        _hitFlashColor = elementColor;
        _hitFlashTimer = 0.3f;
    }

    void Update()
    {
        if (_slowTimer > 0f)
        {
            _slowTimer -= Time.deltaTime;
            if (_slowTimer <= 0f) { SpeedMultiplier = 1f; Sr.color = _baseColor; }
        }

        if (_hitFlashTimer > 0f)
        {
            _hitFlashTimer -= Time.deltaTime;
            bool showElement = Mathf.Sin(_hitFlashTimer * 55f) > 0f;
            Color c = showElement ? _hitFlashColor : _baseColor;
            c.a = Sr.color.a;
            Sr.color = c;
        }
    }

    void ApplySolidSetup()
    {
        _isPhase   = false;
        _baseColor = new Color(1f, 0.35f, 0.35f, 1f);
        Sr.color   = _baseColor;
        SetStrategy(new SolidMoveStrategy());
        Rb.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<CircleCollider2D>().isTrigger = false;
    }

    void ApplyPhaseSetup()
    {
        _isPhase   = true;
        _baseColor = new Color(0.65f, 0.3f, 1f, 1f);
        Sr.color   = _baseColor;
        SetStrategy(new PhaseMoveStrategy());
        Rb.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    void DropItem()
    {
        if (Random.value > dropChance) return;

        int type = Random.Range(0, 3);

        var go = new GameObject("Item");
        go.transform.position   = transform.position;
        go.transform.localScale = Vector3.one * 0.4f;

        go.AddComponent<SpriteRenderer>().sprite = SpriteFactory.CreateCircle(16);

        IItemVisitor visitor = type switch
        {
            0 => new SpeedPotionVisitor(),
            1 => new HealthPotionVisitor(),
            _ => (IItemVisitor)new AtkPotionVisitor(),
        };

        go.AddComponent<ItemPickup>().Init(visitor, _itemColors[type]);
    }

    void OnGUI()
    {
        if (Camera.main == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.75f);
        if (screenPos.z < 0) return;
        screenPos.y = Screen.height - screenPos.y;

        const float W = 60f, H = 10f;
        float x    = screenPos.x - W / 2f;
        float y    = screenPos.y - H / 2f;
        float fill = Mathf.Clamp01((float)_hp / maxHp);

        GUI.color = new Color(0.15f, 0.15f, 0.15f, 0.85f);
        GUI.DrawTexture(new Rect(x, y, W, H), Texture2D.whiteTexture);

        GUI.color = _isPhase ? new Color(0.6f, 0.2f, 1f) : Color.Lerp(Color.red, Color.green, fill);
        GUI.DrawTexture(new Rect(x + 1, y + 1, (W - 2) * fill, H - 2), Texture2D.whiteTexture);

        GUI.color = Color.white;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
            col.gameObject.GetComponent<PlayerController>()?.TakeDamage(20);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            col.GetComponent<PlayerController>()?.TakeDamage(20);
    }
}
