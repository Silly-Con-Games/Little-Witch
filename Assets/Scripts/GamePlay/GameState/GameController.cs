using System.Collections;
using Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public PlayerController witchPrefab;

    public CinemachineVirtualCamera cmCamera;

    public Transform spawnPoint;

    public HUDController hud;

	public PauseController pauseController;

    public MapController mapController;

    public EnemiesController enemiesController;

    public static EGameState GameState 
    { 
        get => internalGS; 
        internal set 
        {
            if(internalGS != value)
            {
               Debug.Log($"Changing gamestate from {internalGS} to {value}");
               internalGS = value;
               onGameStateChanged.Invoke(value);
            }
        } 
    }


    private static EGameState internalGS = EGameState.WaitingForNextWave;
    public static UnityEvent<EGameState> onGameStateChanged = new UnityEvent<EGameState>();

    private PlayerController currentWitch;
    private GlobalConfig conf;


    // Start is called before the first frame update
    void Start()
    {
        internalGS = EGameState.WaitingForNextWave;
        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);
        ApplyConfig();
        StartCoroutine(SpawnWithDelay(0.3f));

        enemiesController.onWaveEnd.AddListener(OnWaveEnd);
        StartCoroutine(WaitAndStartWave(enemiesController.GetCurrentPreperationTime()));
    }

    void ApplyConfig()
    {
        conf = GlobalConfigManager.GetGlobalConfig(); 
    }


    private IEnumerator SpawnWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentWitch = Instantiate(witchPrefab);
        currentWitch.transform.position = spawnPoint.position;
        currentWitch.transform.rotation = spawnPoint.rotation;
        currentWitch.onDeathEvent.AddListener(OnWitchDeath);
        currentWitch.mapController = mapController;
        currentWitch.hudController = hud;
		currentWitch.pauseController = pauseController;

        cmCamera.LookAt = currentWitch.transform;
        cmCamera.Follow = currentWitch.transform;
    }

    private void OnWitchDeath()
    {
        currentWitch.onDeathEvent.RemoveListener(OnWitchDeath);
        currentWitch = null;
        GameState = EGameState.GameOver;
        StartCoroutine(WaitAndRespawnCouroutine());
    }

    private void OnWaveEnd()
    {
        if (enemiesController.WasLastWave())
        {
            GameState = EGameState.GameWon;
        }
        else
        {
            GameState = EGameState.WaitingForNextWave;
            StartCoroutine(WaitAndStartWave(enemiesController.GetCurrentPreperationTime()));
        }
    }

    private IEnumerator WaitAndStartWave(float duration)
    {
        yield return new WaitForSeconds(duration);
        GameState = EGameState.FightingWave;
        enemiesController.SpawnNextWave();
    }

    IEnumerator WaitAndRespawnCouroutine()
    {
        GameState = EGameState.GameOver;
        yield return new WaitForSeconds(conf.respawnTime);
        SceneManager.GetActiveScene();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
