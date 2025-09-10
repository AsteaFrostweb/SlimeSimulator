using NUnit.Framework;
using TMPro;
using UnityEngine;

public class SettingsOption : MonoBehaviour
{
    
    [SerializeField] public string settingKey;

    [SerializeField] private TextMeshProUGUI optionLabel;
    [SerializeField] private TMP_InputField inputField;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(optionLabel == null)
            optionLabel = GetComponentInChildren<TextMeshProUGUI>();

        if(inputField == null)
            inputField = GetComponentInChildren<TMP_InputField>();

        if ((inputField == null) || (optionLabel == null))
        {
            Debug.Log("Unable to setup settings option for gameobject " + gameObject.name + ".");            
            return;
        }

        SettingsOptionsManager.AddOption(settingKey, this);
    }

    public T GetValue<T>() 
    {
        return SettingsOptionsManager.ConvertTo<T>(inputField.text);
    }
    public void SetValue<T>(object _object) 
    {
        string value = SettingsOptionsManager.ConvertTo<string>(_object);
        inputField.text = value;
    }
}
