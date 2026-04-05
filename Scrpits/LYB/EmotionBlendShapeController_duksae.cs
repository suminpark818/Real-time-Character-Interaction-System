using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EmotionBlendShapeController_duksae : MonoBehaviour
{
    [Header("BlendShape 대상 렌더러들")]
    public SkinnedMeshRenderer eye_L;
    public SkinnedMeshRenderer eye_R;
    public SkinnedMeshRenderer eyebrow;
    public SkinnedMeshRenderer eyeline;
    public SkinnedMeshRenderer head;

    [Header("표정 유지 시간 (초)")]
    public float defaultEmotionHoldTime = 2f;

    private Coroutine emotionRoutine;
    private List<SkinnedMeshRenderer> renderers = new();

    private void Awake()
    {
        // 전체 렌더러 리스트 초기화
        if (eye_L != null) renderers.Add(eye_L);
        if (eye_R != null) renderers.Add(eye_R);
        if (eyebrow != null) renderers.Add(eyebrow);
        if (eyeline != null) renderers.Add(eyeline);
        if (head != null) renderers.Add(head);
    }


    public void ApplyEmotion(string emotion)
    {
        if (emotionRoutine != null) StopCoroutine(emotionRoutine);
        emotionRoutine = StartCoroutine(PlayEmotionRoutine(emotion));
    }

    public void ResetExpression()
    {
        if (emotionRoutine != null)
        {
            StopCoroutine(emotionRoutine);
            emotionRoutine = null;
        }
        ResetAllBlendShapes();
        Debug.Log("기본 표정으로 복귀");
    }

    private IEnumerator PlayEmotionRoutine(string emotion)
    {
        ApplyEmotionNow(emotion);
        yield return new WaitForSeconds(defaultEmotionHoldTime);
        ResetAllBlendShapes();
        emotionRoutine = null;
    }

    private void ApplyEmotionNow(string emotion)
    {
        ResetAllBlendShapes();

        foreach (var rend in renderers)
        {
            if (rend == null || rend.sharedMesh == null) continue;

            int index = rend.sharedMesh.GetBlendShapeIndex(emotion);
            if (index >= 0)
                rend.SetBlendShapeWeight(index, 100f);
        }

        Debug.Log($"[EmotionBlendShapeController_duksae] 감정 적용: {emotion}");
    }

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

    private void ResetAllBlendShapes()
    {
        foreach (var rend in renderers)
        {
            if (rend == null || rend.sharedMesh == null) continue;

            var mesh = rend.sharedMesh;
            for (int i = 0; i < mesh.blendShapeCount; i++)
                rend.SetBlendShapeWeight(i, 0f);
        }
    }
}

