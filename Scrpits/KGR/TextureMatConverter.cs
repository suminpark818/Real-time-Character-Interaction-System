using UnityEngine;
using OpenCvSharp;

public static class TextureMatConverter
{
    public static Mat Texture2DToMat(Texture2D input)
    {
        Color32[] pixels = input.GetPixels32();
        byte[] bytes = new byte[pixels.Length * 4];

        for (int i = 0; i < pixels.Length; i++)
        {
            bytes[i * 4 + 0] = pixels[i].b;
            bytes[i * 4 + 1] = pixels[i].g;
            bytes[i * 4 + 2] = pixels[i].r;
            bytes[i * 4 + 3] = pixels[i].a;
        }

        Mat mat = new Mat(input.height, input.width, MatType.CV_8UC4);
        mat.SetArray(0, 0, bytes);
        return mat;
    }

    public static Texture2D MatToTexture2D(Mat mat)
    {
        byte[] bytes = new byte[mat.Width * mat.Height * 4];
        mat.GetArray(0, 0, bytes);

        Color32[] colors = new Color32[mat.Width * mat.Height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color32(
                bytes[i * 4 + 2],
                bytes[i * 4 + 1],
                bytes[i * 4 + 0],
                bytes[i * 4 + 3]
            );
        }

        Texture2D texture = new Texture2D(mat.Width, mat.Height, TextureFormat.RGBA32, false);
        texture.SetPixels32(colors);
        texture.Apply();
        return texture;
    }
}
