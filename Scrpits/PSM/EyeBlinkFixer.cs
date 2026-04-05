using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using System.Collections;
using UnityEngine;

public class EyeBlinkFixer : MonoBehaviour
{
    private CubismParameter eyeL;
    private CubismParameter eyeR;
    private Live2DController live2D;

    private float blinkTimer = 0f;
    private bool isBlinking = false;

    void Start()
    {
        live2D = GetComponent<Live2DController>();
        var model = GetComponent<CubismModel>();
        if (model == null) return;

        eyeL = model.Parameters.FindById("ParamEyeLOpen");
        eyeR = model.Parameters.FindById("ParamEyeROpen");

        if (eyeL == null || eyeR == null)
            Debug.LogWarning("[EyeBlinkFixer] Eye parameters not found!");
    }

    void Update()
    {
        if (isBlinking || eyeL == null || eyeR == null)
            return;

        blinkTimer += Time.deltaTime;
        if (blinkTimer > Random.Range(2.5f, 5.5f))
            StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        isBlinking = true;
        blinkTimer = 0f;

        float closeDur = 0.07f, openDur = 0.1f;
        for (float t = 0; t < closeDur; t += Time.deltaTime)
        {
            float v = Mathf.Lerp(1f, 0f, t / closeDur);
            SetEyes(v);
            yield return null;
        }
        SetEyes(0f);
        yield return new WaitForSeconds(0.05f);

        for (float t = 0; t < openDur; t += Time.deltaTime)
        {
            float v = Mathf.Lerp(0f, 1f, t / openDur);
            SetEyes(v);
            yield return null;
        }
        SetEyes(1f);
        isBlinking = false;
    }

    private void SetEyes(float v)
    {
        if (eyeL != null) { eyeL.Value = v; eyeL.DidChange(true); }
        if (eyeR != null) { eyeR.Value = v; eyeR.DidChange(true); }
        live2D.ApplyNow();
    }
}
