using UnityEngine;
using System.Collections;

public class WebcamInput3D : MonoBehaviour
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

        // 1. OBS 우선 선택
        foreach (var device in devices)
        {
            Debug.Log($"[WebcamInput] 감지된 장치: {device.name}");

            if (device.name.ToLower().Contains("obs"))
            {
                selectedDeviceName = device.name;
                Debug.Log($"[WebcamInput] OBS Virtual Camera 자동 선택됨: {selectedDeviceName}");
                return;
            }
        }

        // 2. OBS가 없으면 사용 가능한 장치 중 재생 가능한 장치 선택
        foreach (var device in devices)
        {
            var tempTex = new WebCamTexture(device.name, 640, 480, 30);
            tempTex.Play();

            if (tempTex.isPlaying)
            {
                selectedDeviceName = device.name;
                tempTex.Stop(); // 임시 테스트 후 종료
                Debug.Log($"[WebcamInput] 정상 장치 선택됨: {selectedDeviceName}");
                return;
            }

            tempTex.Stop();
        }

        Debug.LogError("[WebcamInput] 사용 가능한 장치가 없거나 모두 재생 실패");
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.5f); // 장치 초기화 대기

        if (string.IsNullOrEmpty(selectedDeviceName))
        {
            Debug.LogError("[WebcamInput] 선택된 장치가 없습니다.");
            yield break;
        }

        CamTexture = new WebCamTexture(selectedDeviceName, 640, 480, 30);
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