using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMeshProp : MonoBehaviour, IProp
{
    public float MorphSpeed;
    public List<MeshRenderer> variants;
    public MeshRenderer mesh;
    public BiomeType type;

    private void Awake()
    {
        if (mesh == null)
            mesh = GetComponent<MeshRenderer>();
    }

    public void ResetColors()
    {
        ColorUtils.SetSaturation(mesh, 1f);
    }

    public void Despawn(bool immediate)
    {
        if (immediate)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorApplication.delayCall += () => { if (gameObject) DestroyImmediate(gameObject); };
            else
#endif
                Destroy(gameObject);
            return;
        }

        StartCoroutine(DespawnCoroutine());
    }

    public void Spawn(bool immediate)
    {
        if(variants.Count > 0)
        {
            int index = Random.Range(0,variants.Count);
            mesh = variants[index];
            variants[index].gameObject.SetActive(true);
        }
        ResetColors();
        transform.Rotate(new Vector3(0, Random.Range(0.0f, 360.0f), 0));
        if (immediate)
        {
            transform.localScale = Vector3.one;
            return;
        }

        StartCoroutine(SpawnCoroutine());
    }

    public void Die(bool immediate)
    {
        if (immediate)
        {
            ColorUtils.SetSaturation(mesh, 0f);
            return;
        }

        StartCoroutine(DesaturateCoroutine());
    }

    public void Revive(bool immediate)
    {
        if (immediate)
        {
            ResetColors();
            return;
        }

        StartCoroutine(SaturateCoroutine());
    }

    public EObjectType GetObjectType()
    {
        return EObjectType.Prop;
    }

    public BiomeType GetBiomeType()
    {
        return type;
    }

    IEnumerator SpawnCoroutine()
    {
        transform.localScale = Vector3.zero;

        for (float f = 0f; f < 1f; f += MorphSpeed * Time.deltaTime)
        {
           transform.localScale = Vector3.one * f;
            yield return null;
        }
    }

    IEnumerator DespawnCoroutine()
    {
        Vector3 initSize = mesh.gameObject.transform.localScale;
        for (float f = 1f; f >= 0f; f -= MorphSpeed * Time.deltaTime)
        {
            transform.localScale = Vector3.one * f;
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator DesaturateCoroutine()
    {
        for (float progress = 1f; progress >= 0f; progress -= 0.01f)
        {
            ColorUtils.SetSaturation(mesh, progress);
            yield return null;
        }
    }

    IEnumerator SaturateCoroutine()
    {
        for (float progress = 0f; progress < 1f; progress += 0.01f)
        {
            ColorUtils.SetSaturation(mesh, progress);
            yield return null;
        }
    }
}
