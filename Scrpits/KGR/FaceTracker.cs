using UnityEngine;
using OpenCvSharp;
using System.IO;

public class FaceTracker : WebCamera
{
    public string cascadeFileName = "haarcascade_frontalface_default.xml";
    private CascadeClassifier cascade;
    private Mat grayMat;

    protected override void Start()
    {
        base.Start();

        string cascadePath = Path.Combine(Application.streamingAssetsPath, cascadeFileName);
        cascade = new CascadeClassifier(cascadePath);
    }

    protected override bool ProcessTexture(WebCamTexture inputTexture, ref Texture2D outputTexture)
    {
        // WebCamTexture → Texture2D
        Texture2D tempTex = new Texture2D(inputTexture.width, inputTexture.height, TextureFormat.RGBA32, false);
        tempTex.SetPixels32(inputTexture.GetPixels32());
        tempTex.Apply();

        // Texture2D → Mat
        Mat rgbaMat = TextureMatConverter.Texture2DToMat(tempTex);

        // 색상 변환
        if (grayMat == null || grayMat.Size() != rgbaMat.Size())
        {
            grayMat = new Mat(rgbaMat.Size(), MatType.CV_8UC1);
        }
        Cv2.CvtColor(rgbaMat, grayMat, ColorConversionCodes.BGRA2GRAY);

        // 얼굴 인식
        var faces = cascade.DetectMultiScale(grayMat);
        foreach (var face in faces)
        {
            Cv2.Rectangle(rgbaMat, face, Scalar.Red, 2);
        }

        // Mat → Texture2D
        outputTexture = TextureMatConverter.MatToTexture2D(rgbaMat);
        return true;
    }

}
