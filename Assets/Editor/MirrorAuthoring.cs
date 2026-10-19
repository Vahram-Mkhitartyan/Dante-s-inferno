using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MirrorAuthoring
{
    private const string TrainingScenePath = "Assets/Scenes/training camp.unity";
    private const string TrainingMirrorName = "Sin Mirror";

    [MenuItem("GameObject/Dante's Inferno/Sin Mirror", false, 10)]
    private static void CreateMirror(MenuCommand command)
    {
        GameObject mirror = new GameObject(TrainingMirrorName);
        GameObjectUtility.SetParentAndAlign(mirror, command.context as GameObject);
        Undo.RegisterCreatedObjectUndo(mirror, "Create Sin Mirror");
        mirror.AddComponent<MirrorReflection>();
        Selection.activeGameObject = mirror;
    }

    [MenuItem("Tools/Dante's Inferno/Place Training Mirror")]
    private static void PlaceTrainingMirror()
    {
        if (EditorSceneManager.GetActiveScene().path != TrainingScenePath)
        {
            Debug.LogWarning($"Open {TrainingScenePath} before placing the training mirror.");
            return;
        }

        GameObject existing = GameObject.Find(TrainingMirrorName);
        if (existing != null)
        {
            Selection.activeGameObject = existing;
            return;
        }

        GameObject mirror = new GameObject(TrainingMirrorName);
        mirror.transform.position = new Vector3(143.5f, 1.6f, 0f);
        mirror.AddComponent<MirrorReflection>();
        Undo.RegisterCreatedObjectUndo(mirror, "Place Training Mirror");
        Selection.activeGameObject = mirror;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
    }
}
