using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Live2D.Cubism.Core;

public class AutoComponentConnector : MonoBehaviour
{
    private Live2DController controller;

    void Awake()
    {
        controller = GetComponent<Live2DController>();
        if (controller == null)
        {
            Debug.LogError("Live2DController not found.");
            return;
        }

        CubismParameter[] parameters = GetComponentsInChildren<CubismParameter>();
        Debug.Log($"[AutoConnector] Found {parameters.Length} CubismParameters.");

        HashSet<string> assigned = new HashSet<string>();

        foreach (var param in parameters)
        {
            string name = param.name.ToLower();

            if (name.Contains("eyeballx")) { controller.eyeBallX = param; }
            else if (name.Contains("eyebally")) { controller.eyeBallY = param; }
            else if (name.Contains("mouthopeny")) { controller.mouthOpenY = param; }
            else if (name.Contains("mouthform")) { controller.mouthForm = param; }
            else if (name.Contains("eyelopen")) { controller.eyeLOpen = param; }
            else if (name.Contains("eyeropen")) { controller.eyeROpen = param; }
            else if (name.Contains("eyelsmile")) { controller.eyeLSmile = param; }
            else if (name.Contains("eyersmile")) { controller.eyeRSmile = param; }
            else if (name.Contains("browly")) { controller.browLY = param; }
            else if (name.Contains("browry")) { controller.browRY = param; }
            else if (name.Contains("anglex")) { controller.angleX = param; }
            else if (name.Contains("angley")) { controller.angleY = param; }
            else if (name.Contains("anglez")) { controller.angleZ = param; }
            else
            {
                controller.SetExtraParameter(param.name, param);
                Debug.Log($"[AutoConnector] Extra parameter: {param.name}");
            }
        }
    }
}
