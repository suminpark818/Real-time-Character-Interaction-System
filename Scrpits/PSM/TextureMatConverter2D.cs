using UnityEngine;
using OpenCvSharp;

public static class TextureMatConverter2D
{
    /// <summary>
    /// WebCamTexture∏¶ Mat¿∏∑Œ ∫Ø»Ø
    /// </summary>
    /// <param name="webcamTexture">WebCamTexture ∞¥√º</param>
    /// <returns>OpenCvSharp.Mat ∞¥√º</returns>
    public static Mat WebCamTextureToMat(WebCamTexture webcamTexture)
    {
        // WebCamTexture °Ê Texture2D
        Texture2D tempTexture = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGBA32, false);
        tempTexture.SetPixels32(webcamTexture.GetPixels32());
        tempTexture.Apply();

        // Texture2D °Ê Mat
        return Texture2DToMat(tempTexture);
    }

    /// <summary>
    /// Texture2D∏¶ Mat¿∏∑Œ ∫Ø»Ø
    /// </summary>
    /// <param name="texture">Texture2D ∞¥√º</param>
    /// <returns>OpenCvSharp.Mat ∞¥√º</returns>
    public static Mat Texture2DToMat(Texture2D texture)
    {
        Color32[] pixels = texture.GetPixels32();
        byte[] bytes = new byte[pixels.Length * 4];

        for (int i = 0; i < pixels.Length; i++)
        {
            bytes[i * 4 + 0] = pixels[i].b;
            bytes[i * 4 + 1] = pixels[i].g;
            bytes[i * 4 + 2] = pixels[i].r;
            bytes[i * 4 + 3] = pixels[i].a;
        }

        Mat mat = new Mat(texture.height, texture.width, MatType.CV_8UC4);
        mat.SetArray(0, 0, bytes);
        return mat;
    }
}
