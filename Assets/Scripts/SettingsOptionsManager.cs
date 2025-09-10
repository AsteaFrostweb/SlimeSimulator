using System;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsOptionsManager
{
    public static Dictionary<string, SettingsOption> settingOptions = new Dictionary<string, SettingsOption>();

    public static void AddOption(string key, SettingsOption option) 
    {
        if (!settingOptions.ContainsKey(key))
        {
            settingOptions.Add(key, option);
        }
        else 
        {
            Debug.Log("Unable to register setting with key:" + key + " to the settigsOptions Dictionary.");
        }
    }

    public static T GetValue<T>(string key)
    {
        T t = default(T);
        if (!settingOptions.ContainsKey(key))
        {
            Debug.Log("Unable to find setting with key:" + key + " to the settigsOptions Dictionary.");
            return t;
        }

        SettingsOption option = null;
        settingOptions.TryGetValue(key, out option);
        if (option == null) 
        {
            Debug.Log("Unable to retrieve setting with key:" + key + " to the settigsOptions Dictionary.");
            return t;
        }

        t = option.GetValue<T>();

        return t;
    }
      

    public static void SetValue<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("Cannot set value: key is null or empty.");
            return;
        }

        if (!settingOptions.TryGetValue(key, out var option) || option == null)
        {
            Debug.LogWarning($"Cannot set value: SettingsOption with key '{key}' not found.");
            return;
        }

        // Use the SettingsOption's SetValue method to update the UI
        option.SetValue<T>(value);
    }
    

    public static T ConvertTo<T>(object value)
    {
        try
        {
            if (value is T variable)
                return variable; // Already correct type

            return (T)System.Convert.ChangeType(value, typeof(T));
        }
        catch (Exception ex)
        {
            Debug.Log($"Failed to convert '{value}' to type {typeof(T)}: {ex.Message}");
            return default;
        }
    }

    
}
