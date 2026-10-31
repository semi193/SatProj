using UnityEngine;

public class EmissionFix : MonoBehaviour
{

    private void Awake()
    {
        ApplyEmissionToAllChildren();
    }

    private void ApplyEmissionToAllChildren()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning($"На объекте {gameObject.name} и его детях не найдено ни одного Renderer!");
            return;
        }

        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                mat.EnableKeyword("_EMISSION");

                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

                mat.shader = mat.shader;
            }
        }
    }
}