using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreateColorPickerPrefab
{
    [MenuItem("Tools/Create Color Picker Prefab")]
    public static void CreateColorPicker()
    {
        // Parent ColorPicker object
        GameObject pickerGO = new GameObject("ColorPicker", typeof(RectTransform));
        RectTransform pickerRT = pickerGO.GetComponent<RectTransform>();
        pickerRT.sizeDelta = new Vector2(150, 100);

        ColorPicker colorPicker = pickerGO.AddComponent<ColorPicker>();

        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (!canvas)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        pickerGO.transform.SetParent(canvas.transform, false);

        // Create color preview image at top
        GameObject previewGO = new GameObject("Preview", typeof(RectTransform), typeof(Image));
        RectTransform previewRT = previewGO.GetComponent<RectTransform>();
        previewRT.SetParent(pickerGO.transform, false);
        previewRT.anchorMin = new Vector2(0.5f, 1f);
        previewRT.anchorMax = new Vector2(0.5f, 1f);
        previewRT.pivot = new Vector2(0.5f, 1f);
        previewRT.anchoredPosition = new Vector2(0, 0);
        previewRT.sizeDelta = new Vector2(150, 20);
        Image previewImage = previewGO.GetComponent<Image>();
        previewImage.color = Color.white;

        string[] channels = { "R", "G", "B", "A" };
        Color[] channelColors = { Color.red, Color.green, Color.blue, Color.white };
        float[] yOffsets = { -20, -40, -60, -80 };
        Slider[] sliders = new Slider[4];

        for (int i = 0; i < channels.Length; i++)
        {
            // Instantiate default Slider prefab
            GameObject sliderPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Slider.prefab");
            if (!sliderPrefab)
            {
                Debug.LogError("Cannot find default Slider prefab. Please create manually at Assets/Prefabs/Slider.prefab");
                return;
            }

            GameObject sliderGO = PrefabUtility.InstantiatePrefab(sliderPrefab) as GameObject;
            sliderGO.name = channels[i];
            sliderGO.transform.SetParent(pickerGO.transform, false);

            RectTransform sliderRT = sliderGO.GetComponent<RectTransform>();
            sliderRT.anchorMin = new Vector2(0.5f, 1f);
            sliderRT.anchorMax = new Vector2(0.5f, 1f);
            sliderRT.pivot = new Vector2(0.5f, 1f);
            sliderRT.anchoredPosition = new Vector2(0, yOffsets[i]);
            sliderRT.sizeDelta = new Vector2(150, 20);

            Slider slider = sliderGO.GetComponent<Slider>();
            sliders[i] = slider;

            // Setup slider colors
            SetupSliderColors(slider, channelColors[i]);
        }

        // Assign sliders to ColorPicker fields
        colorPicker.GetType().GetField("redSlider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(colorPicker, sliders[0]);
        colorPicker.GetType().GetField("greenSlider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(colorPicker, sliders[1]);
        colorPicker.GetType().GetField("blueSlider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(colorPicker, sliders[2]);
        colorPicker.GetType().GetField("alphaSlider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(colorPicker, sliders[3]);

        string prefabPath = "Assets/ColorPicker.prefab";
        PrefabUtility.SaveAsPrefabAssetAndConnect(pickerGO, prefabPath, InteractionMode.UserAction);
        Debug.Log("ColorPicker prefab created at " + prefabPath);
    }

    private static void SetupSliderColors(Slider slider, Color color)
    {
        if (!slider) return;

        // Fill image
        Image fillImage = slider.fillRect?.GetComponent<Image>();
        if (fillImage) fillImage.color = color;

        // Background image (keep neutral)
        Image bgImage = slider.GetComponentInChildren<Image>();
        if (bgImage) bgImage.color = Color.gray;

        // Handle color and UI states
        ColorBlock cb = slider.colors;
        cb.normalColor = color;
        cb.highlightedColor = color * 1.2f;
        cb.pressedColor = color * 0.8f;
        cb.selectedColor = color;
        cb.disabledColor = Color.gray;
        slider.colors = cb;

        // Initial value to 1
        slider.value = 1f;
    }
}
