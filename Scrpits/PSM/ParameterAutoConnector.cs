using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.LookAt;
using Live2D.Cubism.Framework.MouthMovement;

public class ParameterAutoConnector : MonoBehaviour
{
    private CubismLookController lookController;
    private CubismMouthController mouthController;
    private MouthFormController mouthFormController;

    private CubismParameter mouthOpenParameter;

    private void Awake()
    {
        AutoAssignControllers();
        AutoAssignParameters();
    }

    /// <summary>
    /// 자동으로 컨트롤러들을 할당합니다.
    /// </summary>
    private void AutoAssignControllers()
    {
        lookController = GetComponentInChildren<CubismLookController>();
        mouthController = GetComponentInChildren<CubismMouthController>();
        mouthFormController = GetComponentInChildren<MouthFormController>();

        if (lookController == null)
        {
            Debug.LogWarning("CubismLookController not found.");
        }

        if (mouthController == null)
        {
            Debug.LogWarning("CubismMouthController not found.");
        }

        if (mouthFormController == null)
        {
            Debug.LogWarning("MouthFormController not found.");
        }

        Debug.Log("Controllers successfully assigned.");
    }

    /// <summary>
    /// 모델의 파라미터들을 자동으로 할당합니다.
    /// </summary>
    private void AutoAssignParameters()
    {
        CubismParameter[] parameters = GetComponentsInChildren<CubismParameter>();

        foreach (CubismParameter param in parameters)
        {
            string paramName = param.name.ToLower();

            // EyeBall X & Y
            if (paramName.Contains("eyeballx") && lookController != null)
            {
                lookController.Target = param.gameObject;
                Debug.Log($"EyeBall X Parameter Connected: {param.name}");
            }
            else if (paramName.Contains("eyebally") && lookController != null)
            {
                lookController.Target = param.gameObject;
                Debug.Log($"EyeBall Y Parameter Connected: {param.name}");
            }

            // Mouth Open
            if (paramName.Contains("mouthopeny"))
            {
                mouthOpenParameter = param;
                Debug.Log($"Mouth Open Parameter Connected: {param.name}");
            }

            // Mouth Form
            if (paramName.Contains("mouthform") && mouthFormController != null)
            {
                mouthFormController.mouthFormParameter = param;
                Debug.Log($"Mouth Form Parameter Connected: {param.name}");
            }
        }

        Debug.Log("Parameter connection complete.");
    }

    /// <summary>
    /// Update mouth opening value.
    /// </summary>
    /// <param name="amount">Mouth open value (0 to 1)</param>
    public void UpdateMouthOpen(float amount)
    {
        if (mouthOpenParameter != null)
        {
            mouthOpenParameter.Value = Mathf.Clamp01(amount);
            Debug.Log($"Mouth Open Updated: {mouthOpenParameter.Value}");
        }
    }
}
