using Assets.Scripts.GameEvents;
using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Tutorial
{
    public class TutorialController : MonoBehaviour
    {
        public PlayerController witchPrefab;
        public PlayerSpawnPoint spawnPoint;
        public CinemachineVirtualCamera cmCamera;
        public HUDController hud;
        public PauseController pauseController;

        public MapController mapController;

        private PlayerController currentWitch;

        // Start is called before the first frame update
        void Start()
        {
            GameController.GameState = EGameState.TutorialStart;
            StartCoroutine(SpawnWithDelay(0));
        }

        public void OnWitchDeath()
        {
            Destroy(currentWitch.gameObject);
            StartCoroutine(SpawnWithDelay(2));
        }

        private IEnumerator SpawnWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            GameEventQueue.QueueEvent(new PlayerRespawnedEvent());
            currentWitch = Instantiate(witchPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            currentWitch.onDeathEvent.AddListener(OnWitchDeath);
            currentWitch.mapController = mapController;
            currentWitch.hudController = hud;
            currentWitch.pauseController = pauseController;

            currentWitch.Initialize();

            cmCamera.LookAt = currentWitch.transform;
            cmCamera.Follow = currentWitch.transform;
        }

        public void LastTestFinished()
        {
            hud.ShowHintText("Tutorial finished");
            StartCoroutine(CoroutineUtils.CallWithDelay(SceneManager.LoadScene, 4, "mainMenu"));
        }
    }
}
