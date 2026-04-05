using UnityEngine;
using Live2D.Cubism.Framework.LookAt;

public class LookAtManager : MonoBehaviour
{
    [SerializeField] private CubismLookController lookController;
    private Vector2 lookTargetPosition;

    /// <summary>
    /// 시선 타겟 좌표 업데이트
    /// </summary>
    /// <param name="targetPosition">월드 좌표계의 시선 타겟</param>
    public void UpdateLookTarget(Vector2 targetPosition)
    {
        lookTargetPosition = targetPosition;

        // Look Controller를 이용해 시선 이동
        if (lookController != null)
        {
            var target = lookController.Target as CubismLookTargetBehaviour;
            if (target != null)
            {
                target.transform.position = new Vector3(lookTargetPosition.x, lookTargetPosition.y, target.transform.position.z);
                Debug.Log($" LookAtManager: Updated Look Target to {lookTargetPosition}");
            }
        }
    }
}
