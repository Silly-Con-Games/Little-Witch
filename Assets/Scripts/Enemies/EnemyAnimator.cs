using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Material hitMaterial;

    private Animator animator;
    private Renderer[] renderers;

    private void Start()
    {
        animator = GetComponent<Animator>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void Move(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void GetHit()
    {
        animator.SetTrigger("GetHit");

        foreach (var renderer in renderers)
        {
            StartCoroutine(ChangeToHitMaterial(renderer));
        }
    }

    // Assumes each renderer only has one material
    private IEnumerator ChangeToHitMaterial(Renderer renderer)
    {
        Material originalMaterial = renderer.material;
        renderer.material = hitMaterial;

        yield return new WaitForSecondsRealtime(.2f);

        renderer.material = originalMaterial;

    }


}
