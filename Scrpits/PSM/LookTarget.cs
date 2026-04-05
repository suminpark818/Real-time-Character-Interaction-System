using UnityEngine;
using Live2D.Cubism.Framework.LookAt;

public class LookTarget : MonoBehaviour, ICubismLookTarget
{
    public bool IsActive()
    {
        return true;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
