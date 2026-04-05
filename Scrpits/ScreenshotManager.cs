using UnityEngine;
using System.IO;

public class ScreenshotManager : MonoBehaviour
{
    public Camera captureCamera;

    public void CaptureSceneWithoutUI()
    {
        int width = Screen.width;
        int height = Screen.height;

        RenderTexture rt = new RenderTexture(width, height, 24);
        captureCamera.targetTexture = rt;

        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        captureCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        string filename = Path.Combine(Application.persistentDataPath, $"scene_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        File.WriteAllBytes(filename, screenShot.EncodeToPNG());

        Debug.Log($"UI 없는 화면 캡처 완료: {filename}");

        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
    }
}

