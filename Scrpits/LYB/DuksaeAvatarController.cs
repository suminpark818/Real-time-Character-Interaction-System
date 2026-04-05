using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DuksaeAvatarController : MonoBehaviour, IAvatarExpressionController
{
    [Header("BlendShape 대상 렌더러들")]
    public SkinnedMeshRenderer eye_L;
    public SkinnedMeshRenderer eye_R;
    public SkinnedMeshRenderer eyebrow;
    public SkinnedMeshRenderer eyeline;
    public SkinnedMeshRenderer head;

    [Header("감정 유지 시간")]
    public float emotionHoldTime = 2f;

    private Coroutine emotionRoutine;
    private List<SkinnedMeshRenderer> renderers = new();

    private void Awake()
    {
        if (eye_L != null) renderers.Add(eye_L);
        if (eye_R != null) renderers.Add(eye_R);
        if (eyebrow != null) renderers.Add(eyebrow);
        if (eyeline != null) renderers.Add(eyeline);
        if (head != null) renderers.Add(head);
    }

    // ------------------------------------------------------------
    // IAvatarExpressionController 인터페이스 구현
    // ------------------------------------------------------------

    public void SetEyes(float value)
    {
        if (value >= 1f) Blink();
        else StopBlink();
    }

    public void SetMouth(float value)
    {
        
    }

    public void SetEmotion(string emotion)
    {
        if (emotionRoutine != null)
            StopCoroutine(emotionRoutine);

        emotionRoutine = StartCoroutine(EmotionRoutine(emotion));
    }

    // ------------------------------------------------------------
    // 감정 처리
    // ------------------------------------------------------------

    private IEnumerator EmotionRoutine(string emotion)
    {
        ApplyEmotionNow(emotion);
        yield return new WaitForSeconds(emotionHoldTime);
        ResetEmotionOnly();
        emotionRoutine = null;
    }

    private void ApplyEmotionNow(string emotion)
    {
        ResetEmotionOnly();

        foreach (var rend in renderers)
        {
            if (rend == null || rend.sharedMesh == null) continue;

            int index = rend.sharedMesh.GetBlendShapeIndex(emotion);
            if (index >= 0)
                rend.SetBlendShapeWeight(index, 100f);
        }

        Debug.Log($"[DuksaeAvatarController] 감정 적용: {emotion}");
    }

    private void ResetEmotionOnly()
    {
        string[] emotionNames = { "happy", "sad", "angry" };

        foreach (var rend in renderers)
        {
            if (rend == null || rend.sharedMesh == null) continue;

            foreach (var e in emotionNames)
            {
                int index = rend.sharedMesh.GetBlendShapeIndex(e);
                if (index >= 0)
                    rend.SetBlendShapeWeight(index, 0f);
            }
        }
    }

    // ------------------------------------------------------------
    // 깜빡임 처리
    // ------------------------------------------------------------

    public void Blink()
    {
        foreach (var rend in renderers)
        {
            if (rend == null || rend.sharedMesh == null) continue;

            int index = rend.sharedMesh.GetBlendShapeIndex("blink");
            if (index >= 0)
                rend.SetBlendShapeWeight(index, 100f);
        }
    }

    public void StopBlink()
    {
        foreach (var rend in renderers)
        {
            if (rend == null || rend.sharedMesh == null) continue;

            int index = rend.sharedMesh.GetBlendShapeIndex("blink");
            if (index >= 0)
                rend.SetBlendShapeWeight(index, 0f);
        }
    }
}


