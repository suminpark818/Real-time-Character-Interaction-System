using UnityEngine;
using System.Collections;

public class WebcamInput : MonoBehaviour
{
    public WebCamTexture CamTexture { get; private set; }
    public bool IsPlaying => CamTexture != null && CamTexture.isPlaying;

    private string selectedDeviceName;

    void Awake()
    {
        Debug.Log("[WebcamInput] Awake() 호출됨");

        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log($"[WebcamInput] 감지된 장치 수: {devices.Length}");

        if (devices.Length == 0)
        {
            Debug.LogError("[WebcamInput] 사용 가능한 웹캠이 없습니다.");
            return;
        }

        // 장치 목록 확인 및 OBS 우선 선택
        foreach (var device in devices)
        {
            Debug.Log($"[WebcamInput] 감지된 장치: {device.name}");

            if (device.name.ToLower().Contains("obs"))
            {
                selectedDeviceName = device.name;
                Debug.Log($"[WebcamInput] OBS Virtual Camera 자동 선택됨: {selectedDeviceName}");
                break;
            }
        }

        // OBS가 없을 경우 첫 번째 장치 선택
        if (string.IsNullOrEmpty(selectedDeviceName))
        {
            selectedDeviceName = devices[0].name;
            Debug.Log($"[WebcamInput] 기본 장치 선택됨: {selectedDeviceName}");
        }
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.5f); // 장치 초기화 대기

        CamTexture = new WebCamTexture(selectedDeviceName);
        CamTexture.Play();
        Debug.Log($"[WebcamInput] Start()에서 CamTexture.Play() 호출됨 (장치: {selectedDeviceName})");

        yield return new WaitForSeconds(1f);

        if (CamTexture != null && CamTexture.isPlaying)
        {
            Debug.Log($"[WebcamInput] 재생 성공: {CamTexture.width}x{CamTexture.height}");
        }
        else
        {
            Debug.LogError($"[WebcamInput] {selectedDeviceName} 에서 CamTexture.Play() 실패 (isPlaying == false)");
        }
    }
}
