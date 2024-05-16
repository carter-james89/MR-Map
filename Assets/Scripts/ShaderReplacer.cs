using UnityEngine;

public class ShaderReplacer : MonoBehaviour
{
    public Shader newShader;

    // This method will be called to replace the shaders
    public void ReplaceShaders()
    {
        if (newShader == null)
        {
            Debug.LogWarning("New shader is not set.");
            return;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material != null)
                {
                    material.shader = newShader;
                }
            }
        }
        Debug.Log("Shader replacement complete.");
    }
}
