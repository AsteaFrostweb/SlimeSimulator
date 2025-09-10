using UnityEditor;
using UnityEngine;
using TMPro;

[CustomEditor(typeof(SettingsOption))]
public class SettingsOptionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector first
        DrawDefaultInspector();

        // Reference the target
        SettingsOption settingsOption = (SettingsOption)target;

        EditorGUILayout.Space();

        // Add the Apply Key button
        if (GUILayout.Button("Apply Key"))
        {
            ApplyKey(settingsOption);
        }
    }

    private void ApplyKey(SettingsOption option)
    {
        if (string.IsNullOrEmpty(option.settingKey))
        {
            Debug.LogWarning("SettingKey is empty. Cannot apply.");
            return;
        }

        // 1. Rename GameObject
        option.gameObject.name = $"SettingsOption({option.settingKey})";

        // 2. Find TMP child named "Label"
        Transform labelTransform = option.transform.Find("Label");
        if (labelTransform != null)
        {
            TextMeshProUGUI labelTMP = labelTransform.GetComponent<TextMeshProUGUI>();
            if (labelTMP != null)
            {
                labelTMP.text = option.settingKey +  ":";
                Debug.Log($"Updated Label text to '{option.settingKey}'.");
            }
            else
            {
                Debug.LogWarning("No TextMeshProUGUI component found on 'Label'.");
            }
        }
        else
        {
            Debug.LogWarning("No child named 'Label' found.");
        }

        // Mark scene as dirty for saving
        EditorUtility.SetDirty(option.gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene()
        );
    }
}
