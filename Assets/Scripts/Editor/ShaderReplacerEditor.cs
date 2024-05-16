using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShaderReplacer))]
public class ShaderReplacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShaderReplacer shaderReplacer = (ShaderReplacer)target;

        if (GUILayout.Button("Replace Shaders"))
        {
            shaderReplacer.ReplaceShaders();
        }
    }
}
