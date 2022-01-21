using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{
    public class HintTrigger : MonoBehaviour
    {
        public float hintDelay = 0;
        public EAbilityType hintToDisplay = EAbilityType.None;

        private void OnTriggerEnter(Collider other)
        {
            var ot = other.GetComponent<IObjectType>();
            if (ot != null && ot.GetObjectType() == EObjectType.Player)
            {
                StartCoroutine(TryDisplayHintCor());
            }
        }

        IEnumerator TryDisplayHintCor()
        {
            yield return new WaitForSeconds(hintDelay);
            HintSpawner.SpawnHint(hintToDisplay);
        }
    }
}