using UnityEngine;
using System.IO;

public class RenderTextureSaver : MonoBehaviour
{
    [Header("Assign the RenderTexture you want to save")]
    public Material material;
    private RenderTexture renderTexture;



    public void SaveRenderTexture()
    {
        renderTexture = material.mainTexture as RenderTexture;
        if (renderTexture == null)
        {
            Debug.LogError("No RenderTexture assigned!");
            return;
        }

        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = previous;

        byte[] bytes = tex.EncodeToJPG();
        Destroy(tex);

        string folder = Application.persistentDataPath;
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        string timeStamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"RenderTexture_{timeStamp}.jpg";
        string fullPath = Path.Combine(folder, fileName);

        File.WriteAllBytes(fullPath, bytes);

        Debug.Log($"RenderTexture saved to: {fullPath}");
    }
}
