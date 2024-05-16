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
                    Texture mainTexture = material.mainTexture; // Preserve the main texture
                    material.shader = newShader;
                    material.mainTexture = mainTexture; // Reassign the preserved texture

                    // Set smoothness and metallic to 0
                    if (material.HasProperty("_Metallic"))
                    {
                        material.SetFloat("_Metallic", 0f);
                    }

                    if (material.HasProperty("_Glossiness"))
                    {
                        material.SetFloat("_Glossiness", 0f);
                    }
                }
            }
        }
        Debug.Log("Shader replacement complete.");
    }
}
