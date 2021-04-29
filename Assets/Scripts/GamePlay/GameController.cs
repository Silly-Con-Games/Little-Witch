using System.Collections;
using Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameController : MonoBehaviour
{
    public PlayerController witchPrefab;

    public CinemachineVirtualCamera cmCamera;

    public Transform spawnPoint;

    private PlayerController currentWitch;
    private GlobalConfig conf;
    // Start is called before the first frame update
    void Start()
    {
        GlobalConfigManager.onConfigChanged.AddListener(ApplyConfig);
        ApplyConfig();
        Spawn();
    }

    void ApplyConfig()
    {
        conf = GlobalConfigManager.GetGlobalConfig(); 
    }

    private void Spawn()
    {
        currentWitch = Instantiate(witchPrefab);
        currentWitch.transform.position = spawnPoint.position;
        currentWitch.transform.rotation = spawnPoint.rotation;
        currentWitch.onDeathEvent.AddListener(OnWitchDeath);

        cmCamera.LookAt = currentWitch.transform;
        cmCamera.Follow = currentWitch.transform;
    }

    private void OnWitchDeath()
    {
        currentWitch.onDeathEvent.RemoveListener(OnWitchDeath);
        currentWitch = null;
        StartCoroutine(WaitAndRespawnCouroutine());
    }

    IEnumerator WaitAndRespawnCouroutine()
    {
        yield return new WaitForSeconds(conf.respawnTime);
        SceneManager.GetActiveScene();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
