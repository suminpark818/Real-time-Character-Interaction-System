using Live2D.Cubism.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionClient : MonoBehaviour
{
    public WebcamInput webcamInput;
    public Live2DController live2D;
    public EmotionDisplayUI emotionDisplayUI;

    private string lastEmotion = "neutral";
    private float blinkTimer = 0f;
    private float blinkInterval = 3f; // 평균 깜빡임 간격 (초)

    [System.Serializable]
    public class EmotionScores
    {
        public float happy;
        public float sad;
        public float angry;
        public float neutral;
    }

    [System.Serializable]
    public class EmotionResult
    {
        public EmotionScores face_scores;
        public EmotionScores audio_scores;
    }

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForSeconds(1f);
        live2D = Object.FindFirstObjectByType<Live2DController>();

        if (live2D == null)
        {
            Debug.LogError("[EmotionClient] Live2DController not found!");
            yield break;
        }

        Debug.Log("[EmotionClient] Initialized successfully.");
        StartCoroutine(DemoEmotionCycle());
    }

    private void Update()
    {
        HandleBlink();
    }

    private void HandleBlink()
    {
        if (live2D == null) return;

        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkInterval)
        {
            StartCoroutine(BlinkRoutine());
            blinkTimer = 0f;
            blinkInterval = Random.Range(2f, 4.5f);
        }
    }

    private IEnumerator BlinkRoutine()
    {
        live2D.SetParameterLerp("ParamEyeLOpen", 0f, 0.05f);
        live2D.SetParameterLerp("ParamEyeROpen", 0f, 0.05f);
        yield return new WaitForSeconds(0.1f);
        live2D.SetParameterLerp("ParamEyeLOpen", 1f, 0.1f);
        live2D.SetParameterLerp("ParamEyeROpen", 1f, 0.1f);
    }

    private IEnumerator DemoEmotionCycle()
    {
        while (true)
        {
            ApplyExpression("happy");
            yield return new WaitForSeconds(3f);
            ApplyExpression("angry");
            yield return new WaitForSeconds(3f);
            ApplyExpression("sad");
            yield return new WaitForSeconds(3f);
            ApplyExpression("neutral");
            yield return new WaitForSeconds(3f);
        }
    }

    public void ApplyExpression(string emotionName)
    {
        EmotionResult fakeResult = new EmotionResult
        {
            face_scores = new EmotionScores(),
            audio_scores = new EmotionScores()
        };

        switch (emotionName.ToLower())
        {
            case "happy": fakeResult.face_scores.happy = 1f; break;
            case "angry": fakeResult.face_scores.angry = 1f; break;
            case "sad": fakeResult.face_scores.sad = 1f; break;
            default: fakeResult.face_scores.neutral = 1f; break;
        }

        ApplyExpression(fakeResult);
    }

    public void ApplyExpression(EmotionResult result)
    {
        string finalEmotion = GetDominantEmotion(result.face_scores, result.audio_scores);
        Debug.Log($"[EmotionClient] 최종 감정: {finalEmotion}");

        if (emotionDisplayUI != null)
            emotionDisplayUI.UpdateFinalEmotion(finalEmotion);

        if (live2D == null)
        {
            Debug.LogError("[EmotionClient] Live2DController is null!");
            return;
        }

        // 감정별 파라미터 값 설정 (lerp로 자연 전환)
        switch (finalEmotion)
        {
            case "happy":
                // 웃는 표정
                live2D.SetParameterLerp("Paramoutertoggle_face1", 1f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face2", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face3", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face4", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face5", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face6", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face7", 0f, 0.15f);
                live2D.SetParameterLerp("ParamCheek", 0.7f, 0.15f);
                break;

            case "angry":
                // 화난 표정
                live2D.SetParameterLerp("Paramoutertoggle_face2", 1f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face1", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face3", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face4", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face5", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face6", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face7", 0f, 0.15f);
                live2D.SetParameterLerp("ParamCheek", 0.4f, 0.15f);
                break;

            case "sad":
                // 슬픈 표정
                live2D.SetParameterLerp("Paramoutertoggle_face3", 1f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face1", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face2", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face4", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face5", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face6", 0f, 0.15f);
                live2D.SetParameterLerp("Paramoutertoggle_face7", 0f, 0.15f);
                live2D.SetParameterLerp("ParamCheek", 0.2f, 0.15f);
                break;

            case "neutral":
            default:
                // 기본 표정 (모두 끔)
                for (int i = 1; i <= 7; i++)
                    live2D.SetParameterLerp($"Paramoutertoggle_face{i}", 0f, 0.15f);
                live2D.SetParameterLerp("ParamCheek", 0f, 0.15f);
                break;
        }
        // 모든 파라미터 변경 후 모델 즉시 업데이트
        live2D.ApplyNow();
        StartCoroutine(ApplySmoothly());


    }


    private string GetDominantEmotion(EmotionScores face, EmotionScores audio)
    {
        Dictionary<string, float> total = new Dictionary<string, float>
        {
            { "happy", face.happy + audio.happy },
            { "sad", face.sad + audio.sad },
            { "angry", face.angry + audio.angry },
            { "neutral", face.neutral + audio.neutral }
        };

        string best = "neutral";
        float max = 0f;

        foreach (var kv in total)
        {
            if (kv.Value > max)
            {
                best = kv.Key;
                max = kv.Value;
            }
        }
        return best;
    }

    private IEnumerator ApplySmoothly()
    {
        for (int i = 0; i < 3; i++) // 0.6초 동안 3회 업데이트
        {
            live2D.ApplyNow();
            yield return new WaitForSeconds(0.2f);
        }
    }

}
