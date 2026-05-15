using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFacade : MonoBehaviour
{
    public static GameFacade Instance { get; private set; }

    private int   _score;
    private float _timeLeft = 60f;
    private bool  _over;

    public bool IsGameOver => _over;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (_over)
        {
            if (Input.GetKeyDown(KeyCode.R)) Restart();
            return;
        }

        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f) GameOver();
    }

    public void AddScore(int pts) => _score += pts;

    public void GameOver()
    {
        if (_over) return;
        _over = true;
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnGUI()
    {
        var bold = new GUIStyle(GUI.skin.label) { fontSize = 44, fontStyle = FontStyle.Bold };
        bold.normal.textColor = Color.white;

        GUI.Label(new Rect(10, 10, 300, 56), $"Score: {_score}", bold);

        float t = Mathf.Max(0f, _timeLeft);
        var centerBold = new GUIStyle(bold) { alignment = TextAnchor.UpperCenter };
        centerBold.normal.textColor = t < 10f ? Color.red : Color.white;
        GUI.Label(new Rect(Screen.width / 2 - 80, 10, 160, 56), $"{Mathf.CeilToInt(t)}s", centerBold);

        if (!_over) return;

        GUI.color = new Color(0f, 0f, 0f, 0.65f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        var big = new GUIStyle(bold) { fontSize = 112, alignment = TextAnchor.MiddleCenter };
        big.normal.textColor = Color.yellow;
        GUI.Label(new Rect(0, Screen.height / 2 - 140, Screen.width, 130), "GAME OVER", big);

        var mid = new GUIStyle(bold) { fontSize = 56, alignment = TextAnchor.MiddleCenter };
        GUI.Label(new Rect(0, Screen.height / 2 + 10, Screen.width, 70), $"Score: {_score}", mid);

        var small = new GUIStyle(bold) { fontSize = 34, alignment = TextAnchor.MiddleCenter };
        small.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
        GUI.Label(new Rect(0, Screen.height / 2 + 95, Screen.width, 50), "Press  R  to restart", small);
    }
}
