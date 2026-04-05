using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Live2DController : MonoBehaviour
{
    private CubismModel model;
    private Dictionary<string, CubismParameter> allParameters;
    private Dictionary<string, CubismParameter> extraParameters = new Dictionary<string, CubismParameter>();
    private Dictionary<string, float> targetValues = new Dictionary<string, float>();
    private bool isUpdating = false;

    [Header("Mouth Parameters")]
    public CubismParameter mouthOpenY;
    public CubismParameter mouthForm;

    [Header("Eye Parameters")]
    public CubismParameter eyeBallX;
    public CubismParameter eyeBallY;

    [Header("Extra Eye Parameters")]
    public CubismParameter eyeLOpen;
    public CubismParameter eyeROpen;
    public CubismParameter eyeLSmile;
    public CubismParameter eyeRSmile;

    [Header("Brow Parameters")]
    public CubismParameter browLY;
    public CubismParameter browRY;

    [Header("Head Rotation Parameters")]
    public CubismParameter angleX;
    public CubismParameter angleY;
    public CubismParameter angleZ;

    private void Awake()
    {
        model = GetComponent<CubismModel>();
        CacheAllParameters();
    }

    private void Start()
    {
        StartCoroutine(BlinkRoutine());
    }

    public void CacheAllParameters()
    {
        if (model == null)
            model = GetComponent<CubismModel>();

        allParameters = model.Parameters.ToDictionary(p => p.name, p => p);
        Debug.Log($"[Live2DController] 총 {allParameters.Count}개의 파라미터 캐싱 완료");
    }

    public CubismParameter FindParameter(string name)
    {
        if (allParameters.TryGetValue(name, out var p))
            return p;
        return null;
    }

    public CubismParameter FindParameterByLooseName(string name)
    {
        foreach (var kvp in allParameters)
        {
            string key = kvp.Key.Replace("Param", "").Trim().ToLower();
            string target = name.Replace("Param", "").Trim().ToLower();
            if (key == target)
                return kvp.Value;
        }
        return null;
    }

    public List<string> GetAllParameterNames()
    {
        if (allParameters == null)
            return new List<string>();
        return new List<string>(allParameters.Keys);
    }

    public void SetParameter(string paramName, float value)
    {
        if (allParameters != null && allParameters.TryGetValue(paramName, out var param))
        {
            param.Value = value;
            param.DidChange(true);
        }
        else
        {
            Debug.LogWarning($"[Live2D] Parameter not found: {paramName}");
        }
    }

    public void SetParameterImmediate(CubismParameter param, float value)
    {
        if (param == null) return;
        param.Value = value;
        param.DidChange(true);
    }

    public void SetParameterImmediate(string paramName, float value)
    {
        if (allParameters != null && allParameters.TryGetValue(paramName, out var p))
        {
            p.Value = value;
            p.DidChange(true);
        }
        else
        {
            Debug.LogWarning($"[Live2DController] Parameter not found for immediate set: {paramName}");
        }
    }

    public void SetParameterLerp(string paramName, float target, float duration = 0.4f)
    {
        if (allParameters == null || !allParameters.ContainsKey(paramName))
        {
            Debug.LogWarning($"[Live2DController] Parameter not found: {paramName}");
            return;
        }

        StartCoroutine(LerpParameter(paramName, target, duration));
    }

    private IEnumerator LerpParameter(string paramName, float target, float duration)
    {
        var p = allParameters[paramName];
        float start = p.Value;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / duration);
            p.Value = Mathf.Lerp(start, target, t);
            p.DidChange(true);
            yield return null;
        }

        p.Value = target;
        p.DidChange(true);
        ApplyNow();
    }


    private IEnumerator UpdateParameters(float speed)
    {
        isUpdating = true;
        while (targetValues.Count > 0)
        {
            foreach (var kv in targetValues.ToList())
            {
                CubismParameter param = null;

                if (allParameters != null && allParameters.TryGetValue(kv.Key, out var found))
                    param = found;
                else if (extraParameters != null && extraParameters.TryGetValue(kv.Key, out var extra))
                    param = extra;

                if (param == null)
                {
                    targetValues.Remove(kv.Key);
                    continue;
                }

                float current = param.Value;
                float target = kv.Value;
                float next = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
                param.Value = next;
                param.DidChange(true);

                if (Mathf.Abs(next - target) < 0.01f)
                {
                    param.Value = target;
                    param.DidChange(true);
                    targetValues.Remove(kv.Key);
                }
            }
            yield return null;
        }
        isUpdating = false;
    }

    public void UpdateMouth(float amount)
    {
        if (mouthOpenY != null)
        {
            mouthOpenY.Value = Mathf.Clamp01(amount);
            mouthOpenY.DidChange(true);
        }
    }

    public void UpdateEye(float x, float y)
    {
        if (eyeBallX != null)
        {
            eyeBallX.Value = Mathf.Clamp(x, -1f, 1f);
            eyeBallX.DidChange(true);
        }
        if (eyeBallY != null)
        {
            eyeBallY.Value = Mathf.Clamp(y, -1f, 1f);
            eyeBallY.DidChange(true);
        }
    }

    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));
            float durationClose = 0.1f;
            float timer = 0f;
            while (timer < durationClose)
            {
                SetEyeOpen(Mathf.Lerp(1f, 0f, timer / durationClose));
                timer += Time.deltaTime;
                yield return null;
            }
            SetEyeOpen(0f);
            yield return new WaitForSeconds(0.05f);
            float durationOpen = 0.15f;
            timer = 0f;
            while (timer < durationOpen)
            {
                SetEyeOpen(Mathf.Lerp(0f, 1f, timer / durationOpen));
                timer += Time.deltaTime;
                yield return null;
            }
            SetEyeOpen(1f);
        }
    }

    public void SetEyeOpen(float value)
    {
        if (eyeLOpen == null)
            eyeLOpen = allParameters.Values.FirstOrDefault(p => p.name.ToLower().Contains("left eye blink"));
        if (eyeROpen == null)
            eyeROpen = allParameters.Values.FirstOrDefault(p => p.name.ToLower().Contains("right eye blink"));

        if (eyeLOpen != null)
        {
            eyeLOpen.Value = Mathf.Clamp01(value);
            eyeLOpen.DidChange(true);
        }
        if (eyeROpen != null)
        {
            eyeROpen.Value = Mathf.Clamp01(value);
            eyeROpen.DidChange(true);
        }
    }

    public void ApplyNow()
    {
        if (model == null)
            model = GetComponent<CubismModel>();

        if (model == null)
        {
            Debug.LogWarning("[Live2DController] CubismModel not found.");
            return;
        }

        // 강제 업데이트만 수행 (ForceDraw 제거)
        model.ForceUpdateNow();


    }


    // --- Extra Parameter Direct Control (for AutoConnector, FaceTracker, SetupLive2D) ---

    public void SetExtraParameter(string key, CubismParameter param)
    {
        if (param == null) return;
        if (!extraParameters.ContainsKey(key))
        {
            extraParameters.Add(key, param);
            Debug.Log($"[Live2DController] Extra parameter added: {key} -> {param.name}");
        }
    }

    public void SetExtraParameterValue(string key, float value)
    {
        if (extraParameters.ContainsKey(key))
        {
            var p = extraParameters[key];
            p.Value = value;
            p.DidChange(true);
        }
        else if (allParameters.ContainsKey(key))
        {
            var p = allParameters[key];
            p.Value = value;
            p.DidChange(true);
        }
        else
        {
            Debug.LogWarning($"[Live2DController] Parameter not found: {key}");
        }
    }

    public void UpdateHeadRotation(float x, float y, float z)
    {
        if (angleX != null)
        {
            angleX.Value = x;
            angleX.DidChange(true);
        }
        if (angleY != null)
        {
            angleY.Value = y;
            angleY.DidChange(true);
        }
        if (angleZ != null)
        {
            angleZ.Value = z;
            angleZ.DidChange(true);
        }

        // 추가적으로 body 관련 파라미터 동기화
        foreach (var kv in extraParameters)
        {
            string key = kv.Key.ToLower();
            var p = kv.Value;

            if (key.Contains("bodyanglex")) p.Value = x;
            else if (key.Contains("bodyangley")) p.Value = y;
            else if (key.Contains("bodyanglez")) p.Value = z;
        }
    }
    public void ApplyEmotion(string emotion)
    {
        ResetEmotionParams();

        switch (emotion)
        {
            case "happy":
                SetParameterImmediate("face1_smile1", 1f);
                SetParameterImmediate("face_plus_twinkle", 1f);
                break;
            case "sad":
                SetParameterImmediate("face_plus_tears", 1f);
                SetParameterImmediate("face_plus_shadow", 1f);
                break;
            case "angry":
                SetParameterImmediate("face_plus_panic", 1f);
                break;
            default: // neutral
                break;
        }

        ApplyNow();
    }

    private void ResetEmotionParams()
    {
        string[] emotionParams = {
        "face1_smile1", "face_plus_twinkle", "face_plus_tears",
        "face_plus_shadow", "face_plus_panic"
    };

        foreach (var name in emotionParams)
        {
            if (allParameters.ContainsKey(name))
            {
                allParameters[name].Value = 0f;
                allParameters[name].DidChange(true);
            }
        }
    }

}
