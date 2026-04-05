using UnityEngine;
using UnityEngine.UI;

public class WebcamManager : MonoBehaviour
{
    public RawImage webcamDisplay;         // UI 상에 표시할 이미지
    public AspectRatioFitter aspectFitter; // 비율 자동 조정용 (선택)

    private WebCamTexture webcamTexture;
    private WebCamDevice? selectedDevice;

    void Start()
    {
        StartWebcam();
    }

    public void StartWebcam()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            WebCamDevice device = WebCamTexture.devices[0]; // 첫 번째 웹캠 (노트북 기본 내장 웹캠)
            webcamTexture = new WebCamTexture(device.name);
            webcamDisplay.texture = webcamTexture;
            webcamDisplay.material.mainTexture = webcamTexture;
            webcamTexture.Play();

            if (aspectFitter != null)
            {
                aspectFitter.aspectRatio = (float)webcamTexture.width / webcamTexture.height;
            }

            Debug.Log("웹캠 시작됨: " + device.name);
        }
        else
        {
            Debug.LogError("웹캠 장치가 감지되지 않았습니다.");
        }
    }

    public Texture2D CaptureFaceImage()
    {
        if (webcamTexture == null || !webcamTexture.isPlaying)
        {
            Debug.LogWarning("웹캠이 실행 중이 아닙니다.");
            return null;
        }

        Texture2D image = new Texture2D(webcamTexture.width, webcamTexture.height);
        image.SetPixels(webcamTexture.GetPixels());
        image.Apply();
        return image;
    }

    public void StopWebcam()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
            Debug.Log("웹캠 중지됨");
        }
    }
}

