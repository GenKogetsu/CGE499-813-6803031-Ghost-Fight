using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private GameObject    _scatterPrefab;
    [SerializeField] private GameObject    _heavyPrefab;
    [SerializeField] private Transform     _firePoint;
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private float         _laserChargeRate = 0.8f;

    private enum GunType     { Scatter, Heavy, Laser }
    private enum ElementType { Fire, Ice, Thunder }

    private GunType      _gunType = GunType.Scatter;
    private ElementType  _element = ElementType.Fire;
    private IWeapon      _weapon;
    private LineRenderer _lineRenderer;
    private LaserGun     _laserGunRef;

    private readonly Color[] _elementColors =
    {
        new Color(1f,    0.35f, 0.05f),
        new Color(0.2f,  0.6f,  1f),
        new Color(0.85f, 1f,    0.1f),
    };

    void Awake()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.material      = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.enabled       = false;

        RebuildWeapon();
    }

    void Update()
    {
        RotateToMouse();
        HandleInput();
        FireTick();
    }

    void HandleInput()
    {
        bool changed = false;

        if (Input.GetKeyDown(KeyCode.Q)) { _gunType = GunType.Scatter; changed = true; }
        if (Input.GetKeyDown(KeyCode.E)) { _gunType = GunType.Heavy;   changed = true; }
        if (Input.GetKeyDown(KeyCode.R)) { _gunType = GunType.Laser;   changed = true; }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { _element = ElementType.Fire;    changed = true; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { _element = ElementType.Ice;     changed = true; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { _element = ElementType.Thunder; changed = true; }

        if (changed) RebuildWeapon();
    }

    void RebuildWeapon()
    {
        _laserGunRef = null;

        IWeapon baseGun;
        if (_gunType == GunType.Laser)
        {
            _laserGunRef = new LaserGun(_firePoint, _lineRenderer, _laserChargeRate);
            baseGun = _laserGunRef;
        }
        else
        {
            baseGun = _gunType switch
            {
                GunType.Scatter => new ScatterGun(_scatterPrefab, _firePoint),
                GunType.Heavy   => new HeavyGun(_heavyPrefab,    _firePoint),
                _               => new ScatterGun(_scatterPrefab, _firePoint)
            };
        }

        _weapon = _element switch
        {
            ElementType.Fire    => new FireDecorator(baseGun),
            ElementType.Ice     => new IceDecorator(baseGun),
            ElementType.Thunder => new ThunderDecorator(baseGun),
            _                   => baseGun
        };

        if (_sr != null)           _sr.color = _elementColors[(int)_element];
        if (_lineRenderer != null) _lineRenderer.enabled = false;
    }

    void FireTick()
    {
        if (_weapon == null || _firePoint == null) return;
        _weapon.Tick(
            _firePoint.position,
            GetMouseDir(),
            BuildConfig(),
            Input.GetMouseButton(0));
    }

    void RotateToMouse()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir   = mouse - transform.parent.position;
        float angle   = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    Vector2 GetMouseDir()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return ((Vector2)mouse - (Vector2)transform.parent.position).normalized;
    }

    BulletConfig BuildConfig()
    {
        var player = transform.parent.GetComponent<PlayerController>();
        return new BulletConfig
        {
            damage         = 1f * (player ? player.AttackMultiplier    : 1f),
            speed          = 12f,
            scale          = 0.25f * (player ? player.BulletSizeMultiplier : 1f),
            color          = Color.white,
            applySlowOnHit = false,
            slowFactor     = 0.4f,
            slowDuration   = 2f
        };
    }

    void OnGUI()
    {
        if (GameFacade.Instance != null && GameFacade.Instance.IsGameOver) return;

        string gunName  = _gunType  switch { GunType.Scatter => "Scatter", GunType.Heavy => "Heavy", GunType.Laser => "Laser", _ => "?" };
        string elemName = _element  switch { ElementType.Fire => "Fire", ElementType.Ice => "Ice", ElementType.Thunder => "Thunder", _ => "?" };

        var style = new GUIStyle(GUI.skin.label) { fontSize = 34, fontStyle = FontStyle.Bold };
        style.normal.textColor = _elementColors[(int)_element];

        GUI.Label(new Rect(Screen.width - 370, 10, 360, 48), $"[Q/E/R]  {gunName}",  style);
        GUI.Label(new Rect(Screen.width - 370, 58, 360, 48), $"[1/2/3]  {elemName}", style);

        if (_gunType == GunType.Laser && _laserGunRef != null)
        {
            float charge = _laserGunRef.ChargePercent;
            const float W = 220f, H = 22f;
            float x = Screen.width - 370f;
            float y = 112f;

            GUI.color = new Color(0.15f, 0.15f, 0.15f, 0.85f);
            GUI.DrawTexture(new Rect(x, y, W, H), Texture2D.whiteTexture);

            GUI.color = Color.Lerp(new Color(1f, 0.6f, 0.1f), Color.white, charge);
            GUI.DrawTexture(new Rect(x + 1, y + 1, (W - 2) * charge, H - 2), Texture2D.whiteTexture);

            GUI.color = Color.white;
            var small = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
            small.normal.textColor = Color.white;
            GUI.Label(new Rect(x + W + 8, y, 120f, H + 4), $"Charge {Mathf.RoundToInt(charge * 100)}%", small);
        }
    }
}
