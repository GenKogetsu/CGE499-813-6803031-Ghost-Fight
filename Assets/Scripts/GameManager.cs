using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int   _score;
    private float _timeLeft = 60f;
    private bool  _over;

    void Awake() => Instance = this;

    void Update()
    {
        if (_over)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(0);
            }
            return;
        }

        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f) GameOver();
    }

    public void AddScore(int pts) => _score += pts;

    public void GameOver()
    {
        _over = true;
        Time.timeScale = 0f;
    }

    void OnGUI()
    {
        var style = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold };
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(10, 10, 250, 35), $"Score: {_score}", style);
        GUI.Label(new Rect(10, 48, 250, 35), $"Time:  {Mathf.CeilToInt(Mathf.Max(0, _timeLeft))}s", style);

        if (!_over) return;

        var bigStyle = new GUIStyle(style) { fontSize = 40, alignment = TextAnchor.MiddleCenter };
        bigStyle.normal.textColor = Color.yellow;
        GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 60, 400, 60),
                  "GAME OVER", bigStyle);
        GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2 + 10, 400, 40),
                  $"Final Score: {_score}", style);

        var smallStyle = new GUIStyle(style) { fontSize = 16 };
        smallStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
        GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 55, 200, 30),
                  "Press R to restart", smallStyle);
    }
}
