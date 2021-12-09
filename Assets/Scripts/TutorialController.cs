using Assets.Scripts.GameEvents;
using Cinemachine;
using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public PlayerController witchPrefab;
    public SpawnPoint spawnPoint;
    public CinemachineVirtualCamera cmCamera;
    public HUDController hud;
    public PauseController pauseController;

    public MapController mapController;


    private PlayerController currentWitch;
    private GlobalConfig conf;


    // Start is called before the first frame update
    void Start()
    {
        GameController.GameState = EGameState.TutorialStart;
        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);
        ApplyConfig();
        StartCoroutine(SpawnWithDelay(0.1f));
    }

    void ApplyConfig()
    {
        conf = GlobalConfigManager.GetGlobalConfig();
    }

    public void OnWitchDeath()
    {
        Destroy(currentWitch.gameObject);
        StartCoroutine(SpawnWithDelay(2));
    }

    private IEnumerator SpawnWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentWitch = Instantiate(witchPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        currentWitch.onDeathEvent.AddListener(OnWitchDeath);
        currentWitch.mapController = mapController;
        currentWitch.hudController = hud;
        currentWitch.pauseController = pauseController;

        currentWitch.Initialize();

        cmCamera.LookAt = currentWitch.transform;
        cmCamera.Follow = currentWitch.transform;
    }

}
