using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [Tooltip("How many frames to average before updating FPS")]
    public int frameBatchSize = 60; // e.g., 60 frames ≈ 1 update per second at 60fps

    private int[] fpsBuffer;
    private int fpsBufferIndex;
    private int averageFPS;
    private TextMeshProUGUI outputText;

    void Awake()
    {
        InitializeBuffer();
    }

    void Start()
    {
        outputText = GetComponent<TextMeshProUGUI>();
    }

    void InitializeBuffer()
    {
        if (frameBatchSize <= 0) frameBatchSize = 1;
        fpsBuffer = new int[frameBatchSize];
        fpsBufferIndex = 0;
    }

    void Update()
    {
        // Make sure buffer size is correct if changed at runtime
        if (fpsBuffer == null || fpsBuffer.Length != frameBatchSize)
        {
            InitializeBuffer();
        }

        // Store current frame's FPS in the buffer
        fpsBuffer[fpsBufferIndex++] = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);

        // If we've collected enough samples, calculate and update the display
        if (fpsBufferIndex >= frameBatchSize)
        {
            fpsBufferIndex = 0;
            CalculateAverageFPS();
            outputText.text = $"FPS: {averageFPS}";
        }
    }

    void CalculateAverageFPS()
    {
        int sum = 0;
        for (int i = 0; i < frameBatchSize; i++)
        {
            sum += fpsBuffer[i];
        }
        averageFPS = sum / frameBatchSize;
    }

    public int GetFPS()
    {
        return averageFPS;
    }
}
