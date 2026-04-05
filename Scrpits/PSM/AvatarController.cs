using UnityEngine;

public class AvatarController : MonoBehaviour
{
    public Transform headBone;
    public Transform leftEyeBone;
    public Transform rightEyeBone;

    public void UpdateFace(Vector2 headRotation, Vector2 eyeRotation)
    {
        // 고개 회전 (X, Y)
        headBone.localRotation = Quaternion.Euler(headRotation.y, headRotation.x, 0);

        // 눈 회전 (X, Y)
        leftEyeBone.localRotation = Quaternion.Euler(eyeRotation.y, eyeRotation.x, 0);
        rightEyeBone.localRotation = Quaternion.Euler(eyeRotation.y, eyeRotation.x, 0);
    }

    public void ApplyBlendShapes(float mouthOpen, float browRaise, float smile)
    {
        SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>();

        if (renderer != null)
        {
            renderer.SetBlendShapeWeight(0, mouthOpen * 100);
            renderer.SetBlendShapeWeight(1, browRaise * 100);
            renderer.SetBlendShapeWeight(2, smile * 100);
        }
    }
}
