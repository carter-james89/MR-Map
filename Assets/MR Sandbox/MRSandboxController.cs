using UnityEngine;
using System.Collections.Generic;

public class MRSandboxController : MonoBehaviour
{
    [SerializeField]
    private Shader _stencilShader;

    private List<Material> _stencilMaterials = new List<Material>();

    private void Awake()
    {
        // Ensure the stencil shader is assigned
        if (_stencilShader == null)
        {
            Debug.LogError("Stencil shader is not assigned.");
            return;
        }

        // Apply the stencil shader to all MeshRenderers
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            if (renderer != null && renderer.material != null)
            {
                Material newMaterial = new Material(_stencilShader);
                newMaterial.CopyPropertiesFromMaterial(renderer.material);
                renderer.material = newMaterial;
                _stencilMaterials.Add(newMaterial);
            }
        }

        ToggleSandbox(true);
    }

    public void ToggleSandbox(bool enable)
    {
        foreach (var material in _stencilMaterials)
        {
            if (material != null)
            {
                material.SetInt("_StencilComp", enable ? (int)UnityEngine.Rendering.CompareFunction.Equal : (int)UnityEngine.Rendering.CompareFunction.Always);
            }
        }
    }
}
