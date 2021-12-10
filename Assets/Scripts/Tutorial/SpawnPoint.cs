using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Tutorial
{
    public class SpawnPoint : MonoBehaviour
    {

        public Material active;
        public Material inactive;
        public MeshRenderer mesh;
        public Collider coll;
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
            controller.spawnPoint.Deactivate();
            coll.enabled = false;
            mesh.material = active;
            controller.spawnPoint = this;
        }

        public void Deactivate()
        {
            coll.enabled = true;
            mesh.material = inactive;
        }
    }
}