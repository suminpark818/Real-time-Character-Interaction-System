using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.LookAt;
using Live2D.Cubism.Framework.MotionFade;
using Live2D.Cubism.Framework.MouthMovement;
using Live2D.Cubism.Framework.Pose;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CubismModel))]
public class SetupLive2D : MonoBehaviour
{
    private CubismModel model;

    void Awake()
    {
        model = GetComponent<CubismModel>();
        if (model == null)
        {
            Debug.LogError("[SetupLive2D] CubismModel not found.");
            return;
        }

        Debug.Log("[SetupLive2D] Initializing Live2D Avatar...");

        // Animator 끄기 (모션 덮어쓰기 방지)
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
            Debug.Log("[SetupLive2D] Animator disabled.");
        }

        // 필수 컴포넌트 자동 추가
        AddIfMissing<Live2DController>();
        AddIfMissing<AutoComponentConnector>();
        AddIfMissing<ParameterAutoConnector>();
        AddIfMissing<CubismLookController>();
        AddIfMissing<MouthFormController>();
        AddIfMissing<FaceTracker2D>();
        AddIfMissing<WebcamInput>();
        AddIfMissing<EmotionClient>();
        AddIfMissing<EyeBlinkFixer>();
        AddIfMissing<CubismFadeController>();
        AddIfMissing<CubismExpressionController>();
        AddIfMissing<CubismParameterStore>();
        AddIfMissing<CubismPoseController>();
        AddIfMissing<CubismMouthController>();

        //  PhysicsController 제거 (physics3.json 없음)
        RemovePhysicsControllerIfNoPhysics();


        AssignLookTarget();
        ConnectFaceTracker();
        ConnectEmotionClient();

        var controller = GetComponent<Live2DController>();
        if (controller != null)
            controller.CacheAllParameters();

        ConnectAllParameters(model, controller);

        Debug.Log("[SetupLive2D] Live2D setup complete (no physics mode).");
    }

    private void AddIfMissing<T>() where T : Component
    {
        if (GetComponent<T>() == null)
        {
            gameObject.AddComponent<T>();
            Debug.Log($"[SetupLive2D] {typeof(T).Name} added.");
        }
    }

    private void RemovePhysicsControllerIfNoPhysics()
    {
        var physics = GetComponent<Live2D.Cubism.Framework.Physics.CubismPhysicsController>();
        if (physics == null)
            return;

        // physics3.json이 포함되지 않은 모델이면 항상 null
        var rigField = physics.GetType().GetField("_physicsRig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var rig = rigField?.GetValue(physics);

        if (rig == null)
        {
            DestroyImmediate(physics);
            Debug.Log("[SetupLive2D] Physics3.json not found. CubismPhysicsController removed safely.");
        }
    }

    private void ConfigureEyeBlinkController()
    {
        var eyeBlink = GetComponent<CubismEyeBlinkController>();
        if (eyeBlink == null)
        {
            eyeBlink = gameObject.AddComponent<CubismEyeBlinkController>();
            Debug.Log("[SetupLive2D] EyeBlinkController added manually.");
        }

        eyeBlink.BlendMode = CubismParameterBlendMode.Override;
        eyeBlink.enabled = true;

        var parameters = model.Parameters;
        int attached = 0;

        foreach (var p in parameters)
        {
            string lower = p.name.ToLower();
            if (lower.Contains("eye") && (lower.Contains("blink") || lower.Contains("close") || lower.Contains("open")))
            {
                if (p.GetComponent<CubismEyeBlinkParameter>() == null)
                {
                    p.gameObject.AddComponent<CubismEyeBlinkParameter>();
                    attached++;
                    Debug.Log($"[SetupLive2D] EyeBlink parameter attached: {p.name}");
                }
            }
        }

        if (attached == 0)
            Debug.LogWarning("[SetupLive2D] No EyeBlink parameters found.");
        else
            Debug.Log($"[SetupLive2D] EyeBlink setup complete ({attached} params).");
    }

    private void AssignLookTarget()
    {
        var lookController = GetComponent<CubismLookController>();
        if (lookController == null) return;

        if (lookController.Target == null)
        {
            GameObject target = new GameObject("LookTarget");
            target.transform.SetParent(transform);
            target.transform.localPosition = Vector3.zero;
            var lookTarget = target.AddComponent<LookTarget>();
            lookController.Target = lookTarget;
            Debug.Log("[SetupLive2D] LookTarget auto-created and assigned.");
        }
    }

    private void ConnectFaceTracker()
    {
        var faceTracker = GetComponent<FaceTracker2D>();
        var webcam = GetComponent<WebcamInput>();
        var controller = GetComponent<Live2DController>();

        if (faceTracker != null)
        {
            faceTracker.AssignLive2DController(controller);
            faceTracker.AssignWebcamInput(webcam);
            Debug.Log("[SetupLive2D] FaceTracker2D connected.");
        }
    }

    private void ConnectEmotionClient()
    {
        var emotionClient = GetComponent<EmotionClient>();
        var webcam = GetComponent<WebcamInput>();
        var controller = GetComponent<Live2DController>();
        var ui = FindFirstObjectByType<EmotionDisplayUI>();

        if (emotionClient != null)
        {
            emotionClient.webcamInput = webcam;
            emotionClient.live2D = controller;
            if (ui != null)
            {
                emotionClient.emotionDisplayUI = ui;
                Debug.Log("[SetupLive2D] EmotionClient UI linked.");
            }
            Debug.Log("[SetupLive2D] EmotionClient linked.");
        }
    }

    private void ConnectAllParameters(CubismModel model, Live2DController controller)
    {
        foreach (var param in model.Parameters)
        {
            string id = param.Id.ToLower();
            if (id.Contains("anglex")) controller.angleX = param;
            else if (id.Contains("angley")) controller.angleY = param;
            else if (id.Contains("anglez")) controller.angleZ = param;
            else if (id.Contains("browl")) controller.browLY = param;
            else if (id.Contains("browr")) controller.browRY = param;
            else controller.SetExtraParameter(param.Id, param);
        }
    }
}
