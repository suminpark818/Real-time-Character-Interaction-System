using UnityEngine;

public class EmotionBlendShapeController : MonoBehaviour
{
    [Header("BlendShape 대상 렌더러")]
    public SkinnedMeshRenderer blwRenderer;
    public SkinnedMeshRenderer eyeRenderer;
    public SkinnedMeshRenderer mthRenderer;
    public SkinnedMeshRenderer elRenderer;

    [Header("표정 유지 시간")]
    public float defaultEmotionHoldTime = 2f;

    private Coroutine emotionRoutine;


    private void ResetAllBlendShapes()
    {
        void ResetRenderer(SkinnedMeshRenderer renderer)
        {
            if (renderer == null) return;
            for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
                renderer.SetBlendShapeWeight(i, 0);
        }

        ResetRenderer(blwRenderer);
        ResetRenderer(eyeRenderer);
        ResetRenderer(mthRenderer);
        ResetRenderer(elRenderer);
    }


    public void ApplyEmotion(string emotion)
    {
        if (emotionRoutine != null)
            StopCoroutine(emotionRoutine);

        emotionRoutine = StartCoroutine(PlayEmotionRoutine(emotion));
    }

    private System.Collections.IEnumerator PlayEmotionRoutine(string emotion)
    {
        ApplyEmotionNow(emotion);
        yield return new WaitForSeconds(defaultEmotionHoldTime);
        ResetAllBlendShapes();
    }

    private void ApplyEmotionNow(string emotion)
    {
        ResetAllBlendShapes();

        switch (emotion.ToLower())
        {
            case "happy":
                if (blwRenderer != null) blwRenderer.SetBlendShapeWeight(0, 100); // BLW_SMILE1
                if (eyeRenderer != null) eyeRenderer.SetBlendShapeWeight(0, 100); // EYE_SMILE1
                if (mthRenderer != null) mthRenderer.SetBlendShapeWeight(0, 100); // MTH_SMILE1
                if (elRenderer != null) elRenderer.SetBlendShapeWeight(0, 100);   // EYE_SMILE1
                break;

            case "sad":
                if (blwRenderer != null) blwRenderer.SetBlendShapeWeight(2, 100); // BLW_SAD
                if (eyeRenderer != null) eyeRenderer.SetBlendShapeWeight(2, 100); // EYE_SAD
                if (mthRenderer != null) mthRenderer.SetBlendShapeWeight(2, 100); // MTH_SAD
                if (elRenderer != null) elRenderer.SetBlendShapeWeight(2, 100);   // EYE_SAD
                break;

            case "angry":
                if (blwRenderer != null) blwRenderer.SetBlendShapeWeight(4, 100); // BLW_ANG1
                if (eyeRenderer != null) eyeRenderer.SetBlendShapeWeight(4, 100); // EYE_ANG1
                if (mthRenderer != null) mthRenderer.SetBlendShapeWeight(4, 100); // MTH_ANG1
                if (elRenderer != null) elRenderer.SetBlendShapeWeight(4, 100);   // EYE_ANG1
                break;

            default:
                Debug.LogWarning($"[EmotionBlendShapeController] 알 수 없는 감정: {emotion}");
                break;
        }

        Debug.Log($"[EmotionBlendShapeController] 감정 적용: {emotion}");
    }

    public void ResetExpression()
    {
        if (emotionRoutine != null)
        {
            StopCoroutine(emotionRoutine);
            emotionRoutine = null;
        }

        ResetAllBlendShapes();
        Debug.Log("[EmotionBlendShapeController] 기본 표정으로 복귀");
    }
}
