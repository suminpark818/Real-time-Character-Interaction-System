using UnityEngine;
using OpenCvSharp;
using System.IO;
using FaceRect = OpenCvSharp.Rect;

public class FaceTracker2D : MonoBehaviour
{
    public Live2DController Live2DController;
    public WebcamInput WebcamInput;

    private CascadeClassifier faceCascade;
    private CascadeClassifier eyeCascade;
    private CascadeClassifier mouthCascade;

    private float smoothEyeX = 0f;
    private float smoothEyeY = 0f;
    private float smoothMouth = 0f;
    private float smoothAngleZ = 0f;

    [Range(0f, 1f)] public float smoothing = 0.05f;
    public float deadZone = 0.02f;

    void Start()
    {
        faceCascade = LoadCascade("haarcascade_frontalface_default.xml");
        eyeCascade = LoadCascade("haarcascade_eye.xml");
        mouthCascade = LoadCascade("haarcascade_mcs_mouth.xml");
    }

    private CascadeClassifier LoadCascade(string filename)
    {
        string path = Path.Combine(Application.streamingAssetsPath, filename);
        var cascade = new CascadeClassifier();
        if (!cascade.Load(path))
            Debug.LogWarning($"[FaceTracker2D] Failed to load {filename}");
        return cascade;
    }

    void Update()
    {
        if (WebcamInput == null || !WebcamInput.IsPlaying || Live2DController == null)
            return;

        Mat frame = TextureMatConverter2D.WebCamTextureToMat(WebcamInput.CamTexture);
        if (frame.Empty())
            return;

        FaceRect[] faces = faceCascade.DetectMultiScale(frame);
        if (faces.Length == 0)
        {
            ApplyZeroParameters();
            return;
        }

        FaceRect face = faces[0];
        Mat faceROI = new Mat(frame, face);

        // 추출
        float rawEyeX = CalcEyeX(face, frame);
        float rawEyeY = CalcEyeY(face, frame);
        float rawMouth = CalcMouth(faceROI, face);
        float rawAngleZ = CalcHeadTilt(faceROI);

        // 스무딩 적용
        smoothEyeX = Smooth(smoothEyeX, rawEyeX, smoothing);
        smoothEyeY = Smooth(smoothEyeY, rawEyeY, smoothing);
        smoothMouth = Smooth(smoothMouth, rawMouth, smoothing);
        smoothAngleZ = Smooth(smoothAngleZ, rawAngleZ, smoothing);

        // 데드존 처리
        float finalEyeX = ApplyDeadZone(smoothEyeX);
        float finalEyeY = ApplyDeadZone(smoothEyeY);
        float finalMouth = ApplyDeadZone(smoothMouth);
        float finalAngleZ = ApplyDeadZone(smoothAngleZ);

        // 값 전달
        float angleX = Mathf.Clamp(finalEyeX * 10f, -10f, 10f);
        float angleZ = Mathf.Clamp(finalAngleZ * 0.3f, -10f, 10f);

        Live2DController.UpdateEye(finalEyeX, finalEyeY);
        Live2DController.UpdateMouth(finalMouth);
        Live2DController.UpdateHeadRotation(angleX, 0f, angleZ);
        Live2DController.SetExtraParameterValue("ParamBodyAngleX", angleX);
        Live2DController.SetExtraParameterValue("ParamBodyAngleZ", angleZ);
    }

    private void ApplyZeroParameters()
    {
        smoothEyeX = Smooth(smoothEyeX, 0f, smoothing);
        smoothEyeY = Smooth(smoothEyeY, 0f, smoothing);
        smoothMouth = Smooth(smoothMouth, 0f, smoothing);
        smoothAngleZ = Smooth(smoothAngleZ, 0f, smoothing);

        Live2DController.UpdateEye(0f, 0f);
        Live2DController.UpdateMouth(0f);
        Live2DController.UpdateHeadRotation(0f, 0f, 0f);
        Live2DController.SetExtraParameterValue("ParamBodyAngleX", 0f);
        Live2DController.SetExtraParameterValue("ParamBodyAngleZ", 0f);
    }

    private float CalcEyeX(FaceRect face, Mat frame)
    {
        float centerX = face.X + face.Width / 2f;
        return Mathf.Clamp((centerX - frame.Width / 2f) / (frame.Width / 2f), -1f, 1f) * 2f;
    }

    private float CalcEyeY(FaceRect face, Mat frame)
    {
        float centerY = face.Y + face.Height / 2f;
        return Mathf.Clamp((frame.Height / 2f - centerY) / (frame.Height / 2f), -1f, 1f) * 2f;
    }

    private float CalcMouth(Mat roi, FaceRect face)
    {
        var mouths = mouthCascade.DetectMultiScale(roi);
        if (mouths.Length > 0)
            return Mathf.Clamp01((float)mouths[0].Height / face.Height * 2f);
        return 0f;
    }

    private float CalcHeadTilt(Mat roi)
    {
        var eyes = eyeCascade.DetectMultiScale(roi);
        if (eyes.Length >= 2)
        {
            System.Array.Sort(eyes, (a, b) => a.X.CompareTo(b.X));
            var l = eyes[0];
            var r = eyes[1];
            float dy = r.Y - l.Y;
            float dx = r.X - l.X;
            return Mathf.Clamp(Mathf.Atan2(dy, dx) * Mathf.Rad2Deg, -30f, 30f);
        }
        return 0f;
    }

    private float Smooth(float current, float target, float factor)
    {
        return current + (target - current) * factor;
    }

    private float ApplyDeadZone(float value)
    {
        return Mathf.Abs(value) < deadZone ? 0f : value;
    }

    public void AssignLive2DController(Live2DController controller) => Live2DController = controller;
    public void AssignWebcamInput(WebcamInput input) => WebcamInput = input;
}
