using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    [Header("Assign child sliders for RGBA")]
    [SerializeField] private Slider redSlider;
    [SerializeField] private Slider greenSlider;
    [SerializeField] private Slider blueSlider;
    [SerializeField] private Slider alphaSlider;

    [Header("Preview Image")]
    [SerializeField] private Image previewImage;

    private void Reset()
    {
        if (!redSlider) redSlider = transform.Find("R")?.GetComponent<Slider>();
        if (!greenSlider) greenSlider = transform.Find("G")?.GetComponent<Slider>();
        if (!blueSlider) blueSlider = transform.Find("B")?.GetComponent<Slider>();
        if (!alphaSlider) alphaSlider = transform.Find("A")?.GetComponent<Slider>();
        if (!previewImage) previewImage = transform.Find("Preview")?.GetComponent<Image>();
    }

    private void OnValidate()
    {
        Reset();
    }

    private void Awake()
    {
        // Subscribe to slider changes
        if (redSlider) redSlider.onValueChanged.AddListener(_ => UpdatePreview());
        if (greenSlider) greenSlider.onValueChanged.AddListener(_ => UpdatePreview());
        if (blueSlider) blueSlider.onValueChanged.AddListener(_ => UpdatePreview());
        if (alphaSlider) alphaSlider.onValueChanged.AddListener(_ => UpdatePreview());

        UpdatePreview();
    }

    /// <summary>
    /// Returns the currently selected color, mapping slider values to [min, max] range.
    /// </summary>
    public Color GetColor(float min, float max)
    {
        float r = redSlider ? Mathf.Lerp(min, max, redSlider.value) : min;
        float g = greenSlider ? Mathf.Lerp(min, max, greenSlider.value) : min;
        float b = blueSlider ? Mathf.Lerp(min, max, blueSlider.value) : min;
        float a = alphaSlider ? Mathf.Lerp(min, max, alphaSlider.value) : max;

        return new Color(r, g, b, a);
    }

    /// <summary>
    /// Sets the sliders based on a color in [min, max] range.
    /// </summary>
    public void SetColor(Color color, float min, float max)
    {
        if (redSlider) redSlider.value = Mathf.InverseLerp(min, max, color.r);
        if (greenSlider) greenSlider.value = Mathf.InverseLerp(min, max, color.g);
        if (blueSlider) blueSlider.value = Mathf.InverseLerp(min, max, color.b);
        if (alphaSlider) alphaSlider.value = Mathf.InverseLerp(min, max, color.a);

        UpdatePreview();
    }

    /// <summary>
    /// Updates the preview image color to match the sliders.
    /// </summary>
    private void UpdatePreview()
    {
        if (!previewImage) return;
        previewImage.color = new Color(
            redSlider ? redSlider.value : 0f,
            greenSlider ? greenSlider.value : 0f,
            blueSlider ? blueSlider.value : 0f,
            alphaSlider ? alphaSlider.value : 1f
        );
    }
}
