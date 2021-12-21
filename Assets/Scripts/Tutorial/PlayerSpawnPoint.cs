using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Tutorial
{
    public class PlayerSpawnPoint : MonoBehaviour
    {

        public Material activeMat;
        public Material inactiveMat;
        public MeshRenderer mesh;
        public Collider coll;
        public bool isActive = false;
        private static TutorialController controller;

        private void Awake()
        {
            if (controller == null)
                controller = FindObjectOfType<TutorialController>();
            Assert.IsNotNull(controller, "missing tutorial controller in scene");
        }

        private void OnTriggerEnter(Collider other)
        {
            var ot = other.GetComponent<IObjectType>();
            if (ot != null && ot.GetObjectType() == EObjectType.Player)
                Activate();
        }

        public void Activate()
        {
            if(controller.spawnPoint != this)
            {
                controller.spawnPoint.Deactivate();
                controller.spawnPoint = this;
            }
            isActive = true;
            coll.enabled = false;
            mesh.material = activeMat;
        }

        public void Deactivate()
        {
            coll.enabled = true;
            isActive = false;
            mesh.material = inactiveMat;
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            if (controller == null)
                controller = FindObjectOfType<TutorialController>();
            var prev = controller?.spawnPoint;
            if (isActive)
                Activate();
            else
                Deactivate();
            Undo.RecordObject(prev?.gameObject, "Spawn deactivated");
            Undo.RecordObject(gameObject, "Spawn activated");
            Undo.RecordObject(controller.gameObject, "Red to active spawn updated");
        }
    }
}