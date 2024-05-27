using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(MRRigHand))]
public class HandPoseEditor : Editor
{
    private string poseTitle = "New Pose";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MRRigHand handScript = (MRRigHand)target;

        poseTitle = EditorGUILayout.TextField("Pose Title", poseTitle);

        if (GUILayout.Button("Save Pose"))
        {
            SaveHandPose(handScript, poseTitle);
        }
    }

    private void SaveHandPose(MRRigHand handScript, string title)
    {
        if (handScript._wristRoot == null)
        {
            Debug.LogError("WristRoot is not assigned.");
            return;
        }

        HandPose newPose = ScriptableObject.CreateInstance<HandPose>();
        newPose.title = title;
        newPose.jointRotations.Clear();

        // Add all joint rotations to the hand pose
        foreach (Transform joint in handScript._wristRoot.GetComponentsInChildren<Transform>())
        {
            newPose.jointRotations.Add(joint.localRotation);
        }

        // Ensure directory exists
        string directoryPath = "Assets/HandPoses";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Save the hand pose as a new asset
        string assetPath = Path.Combine(directoryPath, "HandPose_" + title.Replace(" ", "_") + ".asset");
        AssetDatabase.CreateAsset(newPose, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log("Saved new hand pose to " + assetPath);
    }
}
