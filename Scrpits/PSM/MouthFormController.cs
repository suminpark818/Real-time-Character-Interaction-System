using UnityEngine;
using Live2D.Cubism.Core;

public class MouthFormController : MonoBehaviour
{
    public CubismParameter mouthFormParameter;
    public CubismParameter mouthOpenParameter; 

    [Range(0f, 1f)]
    public float openness = 0f;

    [Range(0f, 1f)]
    public float form = 0.5f;

    void Update()
    {
        if (mouthFormParameter != null)
        {
            mouthFormParameter.Value = form;
        }

        if (mouthOpenParameter != null)
        {
            mouthOpenParameter.Value = openness;
        }
    }
}
