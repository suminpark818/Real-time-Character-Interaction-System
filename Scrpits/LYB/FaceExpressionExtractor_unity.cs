using UnityEngine;
using OpenCvSharp.Demo;
using System.Collections.Generic;

public class FaceExpressionExtractor_unity : MonoBehaviour
{
    public FaceDetectorScene detector;
    public MonoBehaviour avatarControllerRaw;
    private IAvatarExpressionController avatarController;

    private float mouthOpenSmoothed = 0f;

    private Queue<float> eyeClosedWindow = new Queue<float>();
    public int windowSize = 4;

    public float blinkCloseThreshold = 0.6f;
    public float blinkOpenThreshold = 0.4f;
    private bool isBlinking = false;

    private float eyeOpenBaseline = 0f;
    private float baselineSum = 0f;
    private int baselineFrames = 0;
    private bool baselineLocked = false;
    public int baselineSampleCount = 100;
    public float eyeSensitivity = 1.6f;

    private float eyeOpenSmoothed = 0f;
    private float lastBlinkTime = 0f;
    public float blinkCooldown = 0.12f;

    void Start()
    {
        if (avatarControllerRaw == null)
        {
            Debug.LogError("avatarControllerRaw is null. Please assign the UnityChan GameObject.");
            return;
        }

        avatarController = avatarControllerRaw.GetComponent<IAvatarExpressionController>();

        if (avatarController == null)
            Debug.LogError("GameObject에는 IAvatarExpressionController를 구현한 컴포넌트가 없습니다!");
        else
            Debug.Log("avatarController 인터페이스 인식 완료!");
    }

    void Update()
    {
        // baseline 리셋 기능
        if (Input.GetKeyDown(KeyCode.R))
        {
            baselineLocked = false;
            baselineSum = 0f;
            baselineFrames = 0;
            Debug.Log("Eye baseline reset");
        }

        if (detector == null || avatarController == null)
            return;

        var processor = detector.GetProcessor();
        if (processor == null || processor.Faces.Count == 0)
            return;

        var points = processor.Faces[0].Marks;
        if (points == null || points.Length < 68)
            return;

        // --- 입 계산 (민감도 완화)
        float rawMouthOpen = Mathf.Clamp01((points[66].Y - points[62].Y) * 5f);
        if (rawMouthOpen < 0.05f) rawMouthOpen = 0f;
        mouthOpenSmoothed = Mathf.Lerp(mouthOpenSmoothed, rawMouthOpen, 0.3f);

        // --- 눈 계산
        float eyeLeftOpen = Mathf.Clamp01((points[41].Y - points[37].Y) * 12f);
        float eyeRightOpen = Mathf.Clamp01((points[47].Y - points[43].Y) * 12f);
        float eyeOpen = (eyeLeftOpen + eyeRightOpen) / 2f;

        eyeOpenSmoothed = Mathf.Lerp(eyeOpenSmoothed, eyeOpen, 0.4f);

        if (!baselineLocked)
        {
            baselineSum += eyeOpenSmoothed;
            baselineFrames++;

            if (baselineFrames >= baselineSampleCount)
            {
                eyeOpenBaseline = baselineSum / baselineFrames;
                baselineLocked = true;
                Debug.Log($"[BASELINE LOCKED] eyeOpenBaseline = {eyeOpenBaseline:F2}");
            }
        }

        float eyeClosedRaw = (eyeOpenBaseline - eyeOpenSmoothed) * eyeSensitivity;
        float eyeClosed = baselineLocked ? Mathf.Clamp(eyeClosedRaw, 0f, 1f) : 0f;

        eyeClosedWindow.Enqueue(eyeClosed > 0.2f ? eyeClosed : 0f);
        if (eyeClosedWindow.Count > windowSize)
            eyeClosedWindow.Dequeue();

        float averageClosed = 0f;
        foreach (float value in eyeClosedWindow)
            averageClosed += value;
        averageClosed /= eyeClosedWindow.Count;

        if (!isBlinking && averageClosed > blinkCloseThreshold && Time.time - lastBlinkTime > blinkCooldown)
        {
            isBlinking = true;
            lastBlinkTime = Time.time;
        }
        else if (isBlinking && averageClosed < blinkOpenThreshold)
        {
            isBlinking = false;
        }

        float eyeBlendValue = isBlinking ? 1f : 0f;

        //Debug.Log($"[EYE DEBUG] open: {eyeOpenSmoothed:F2}, closed: {eyeClosed:F2}, avgClosed: {averageClosed:F2}, baseline: {eyeOpenBaseline:F2}, blinking: {isBlinking}");

        avatarController.SetMouth(mouthOpenSmoothed);
        avatarController.SetEyes(eyeBlendValue);
    }
}


