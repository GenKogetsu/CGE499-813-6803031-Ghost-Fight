using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _baseSpeed    = 5f;
    [SerializeField] private int   _maxHp        = 100;
    [SerializeField] private float _maxStamina   = 100f;
    [SerializeField] private float _staminaDrain = 30f;
    [SerializeField] private float _staminaRegen = 15f;

    public float AttackMultiplier      { get; private set; } = 1f;
    public float BulletSizeMultiplier  { get; private set; } = 1f;

    private Rigidbody2D         _rb;
    private SpriteRenderer      _sr;
    private IPlayerMoveStrategy _moveStrategy;
    private int   _hp;
    private float _stamina;
    private float _speedBoostTimer;
    private bool  _sprinting;

    private static readonly Color _colorNormal = new Color(0.3f, 0.85f, 1f);
    private static readonly Color _colorBoost  = new Color(0f,   0.5f,  1f);

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _rb.gravityScale   = 0f;
        _rb.freezeRotation = true;
        _sr.color = _colorNormal;
        GetComponent<CircleCollider2D>().radius = 0.5f;
    }

    void Start()
    {
        _hp           = _maxHp;
        _stamina      = _maxStamina;
        _moveStrategy = new NormalMoveStrategy();
    }

    void Update()
    {
        TickSpeedBoost();
        TickStamina();
    }

    void FixedUpdate()
    {
        float spd = _baseSpeed;

        if (_speedBoostTimer > 0f)
            spd *= 2f;
        else if (_sprinting)
            spd *= 1.8f;

        _moveStrategy?.Move(_rb, spd);
    }

    public void Accept(IItemVisitor visitor) => visitor?.Visit(this);

    public void ActivateSpeedBoost(float duration)
    {
        _speedBoostTimer = duration;
        _moveStrategy    = new SpeedBoostMoveStrategy();
        _sr.color        = _colorBoost;
    }

    public void Heal(int amount)
    {
        _hp = Mathf.Min(_hp + amount, _maxHp);
    }

    public void BoostAttack()
    {
        AttackMultiplier     += 0.4f;
        BulletSizeMultiplier += 0.15f;
    }

    public void TakeDamage(int dmg)
    {
        _hp -= dmg;
        if (_hp <= 0) GameFacade.Instance?.GameOver();
    }

    void TickStamina()
    {
        bool wantSprint = Input.GetKey(KeyCode.LeftShift) && _speedBoostTimer <= 0f;
        _sprinting = wantSprint && _stamina > 5f;

        if (_sprinting)
            _stamina = Mathf.Max(0f, _stamina - _staminaDrain * Time.deltaTime);
        else
            _stamina = Mathf.Min(_maxStamina, _stamina + _staminaRegen * Time.deltaTime);
    }

    void TickSpeedBoost()
    {
        if (_speedBoostTimer <= 0f) return;

        _speedBoostTimer -= Time.deltaTime;

        if (_speedBoostTimer < 1.5f)
            _sr.color = (Mathf.Sin(Time.time * 12f) > 0) ? _colorBoost : new Color(0.8f, 0.8f, 1f);

        if (_speedBoostTimer <= 0f)
        {
            _moveStrategy = new NormalMoveStrategy();
            _sr.color     = _colorNormal;
        }
    }

    void OnGUI()
    {
        if (GameFacade.Instance != null && GameFacade.Instance.IsGameOver) return;

        const float BAR_W = 380f, BAR_H = 48f, X = 10f;

        float hpY    = Screen.height - 130f;
        float hpFill = Mathf.Clamp01((float)_hp / _maxHp);

        GUI.color = new Color(0.12f, 0.12f, 0.12f, 0.85f);
        GUI.DrawTexture(new Rect(X, hpY, BAR_W, BAR_H), Texture2D.whiteTexture);

        GUI.color = Color.Lerp(Color.red, Color.green, hpFill);
        GUI.DrawTexture(new Rect(X + 2, hpY + 2, (BAR_W - 4) * hpFill, BAR_H - 4), Texture2D.whiteTexture);

        GUI.color = Color.white;
        var barLabel = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 22,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        GUI.Label(new Rect(X, hpY, BAR_W, BAR_H), "HP", barLabel);

        var pctStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 36,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        pctStyle.normal.textColor = Color.Lerp(Color.red, Color.green, hpFill);
        GUI.Label(new Rect(X + BAR_W + 10f, hpY, 120f, BAR_H), $"{Mathf.RoundToInt(hpFill * 100)}%", pctStyle);

        const float ST_H = 28f;
        float stY    = Screen.height - 72f;
        float stFill = Mathf.Clamp01(_stamina / _maxStamina);

        GUI.color = new Color(0.12f, 0.12f, 0.12f, 0.85f);
        GUI.DrawTexture(new Rect(X, stY, BAR_W, ST_H), Texture2D.whiteTexture);

        GUI.color = _sprinting ? new Color(0.1f, 0.8f, 1f) : new Color(0.05f, 0.55f, 0.75f);
        GUI.DrawTexture(new Rect(X + 2, stY + 2, (BAR_W - 4) * stFill, ST_H - 4), Texture2D.whiteTexture);

        GUI.color = Color.white;
        var stLabel = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 17,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        GUI.Label(new Rect(X, stY, BAR_W, ST_H), _sprinting ? "SPRINT" : "Stamina", stLabel);

        var stPct = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 22,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        stPct.normal.textColor = new Color(0.5f, 0.9f, 1f);
        GUI.Label(new Rect(X + BAR_W + 10f, stY, 120f, ST_H), $"{Mathf.RoundToInt(stFill * 100)}%", stPct);

        GUI.color = Color.white;
    }
}
