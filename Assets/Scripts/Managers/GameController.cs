using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
    public float  YToDestroy { get => objectsOutDetector.transform.position.y; }
    public GameObject Lily { get => lily; }
    //public bool IsPlayerOut { get; private set; }

    [Header("References")]
    [SerializeField]
    private GameObject objectsOutDetector;
    [SerializeField]
    private GameObject lily;
    [SerializeField]
    private Timer timer;

    [Header("Properties")]
    private float gameoverDelay = 1f;

    private Player[] _players; 
    private bool _isGamePaused = false;
    public string sceneName;

    //------------------------------------ lifecycle

    protected override void Awake()
    {
        base.Awake();
        _players = new Player[2];
    }

    //------------------------------------ registering objects

    public void RegisterPlayer(Player player)
    {

        if (player.PlayerType == Player.PlayerNumber.Player1)
        {
            _players[0] = player;
        }
            
        else if (player.PlayerType == Player.PlayerNumber.Player2)
        {
            _players[1] = player;
        }
            
    }

    //------------------------------------ pause
    public void PauseButtonPressed()
    {
        Debug.Log("pause pressed");
        if (_isGamePaused) UnpauseGame();
        else PauseGame();
    }
    private void PauseGame()
    {
        if (!_isGamePaused)
        {
            Debug.Log("pause game called");
            _isGamePaused = true;
            Time.timeScale = 0;
            UIManager.Instance.HideGamePanel();
            UIManager.Instance.ShowPausePanel();
        }
    }
    private void UnpauseGame()
    {
        if (_isGamePaused)
        {
            UIManager.Instance.ShowGamePanel();
            UIManager.Instance.HidePausePanel();
            Time.timeScale = 1;
            _isGamePaused = false;
        }
    }
    //------------------------------------ gameover detection
    public void PlayerOut(Player player)
    {
        Debug.Log("game controller: player out");
        if (player.IsAlive)
        {
            player.IsAlive = false;
            if (!_players[0].IsAlive && !_players[1].IsAlive)
                StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        float time = timer.CurrentTimer;
        float bestTime = DataManager.Instance.ReadBestTime();

        if(time > bestTime)
        {
            bestTime = time;
            DataManager.Instance.WriteBestTime(bestTime);
        }
        
        yield return new WaitForSeconds(gameoverDelay);
        Spawner.Instance.SetActiveSpawn(false);
        UIManager.Instance.HideGamePanel();
        UIManager.Instance.ShowGameOverPanel(timer.GetCurrentFormattedTimer(),timer.GetFormattedTimer(bestTime+1));
    }

    //------------------------------------ scenes handling

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }
}
