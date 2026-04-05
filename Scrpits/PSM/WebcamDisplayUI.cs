using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WebcamDisplayUI : MonoBehaviour
{
    public RawImage webcamRawImage;
    public AspectRatioFitter aspectFitter;
    private WebcamInput webcamInput;

    IEnumerator Start()
    {
        // WebcamInput이 준비될 때까지 기다림
        while (webcamInput == null || webcamInput.CamTexture == null || !webcamInput.IsPlaying)
        {
            webcamInput = FindObjectOfType<WebcamInput>();
            yield return null; // 다음 프레임까지 대기
        }

        // CamTexture를 RawImage에 연결
        webcamRawImage.texture = webcamInput.CamTexture;
        webcamRawImage.material.mainTexture = webcamInput.CamTexture;

        if (aspectFitter != null)
        {
            aspectFitter.aspectRatio = (float)webcamInput.CamTexture.width / webcamInput.CamTexture.height;
        }

        Debug.Log("[WebcamDisplayUI] 웹캠 화면 UI에 연결됨");
    }
}
