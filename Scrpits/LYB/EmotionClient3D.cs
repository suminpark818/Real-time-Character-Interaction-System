using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class EmotionClient3D : MonoBehaviour
{
    [Header("입력 소스")]
    public WebcamInput3D webcamInput;
    public MonoBehaviour faceExpressionExtractor;  // 실시간 눈/입 트래킹 스크립트
    public DuksaeAvatarController avatarController;



    [Header("설정")]
    public float interval = 5f;
    public float expressionDuration = 2f;
    public string serverUrl = "http://localhost:5000/analyze";

    private AudioClip currentAudioClip;
    private bool isEmotionPlaying = false;

    private void Start()
    {
        StartCoroutine(SendEmotionRoutine());
    }

    IEnumerator SendEmotionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            if (!webcamInput.IsPlaying || webcamInput.CamTexture == null || isEmotionPlaying)
            {
                continue;
            }

            byte[] imageBytes = GetCurrentImagePNG();

            string micName = Microphone.devices.Length > 0 ? Microphone.devices[0] : null;
            if (micName == null)
            {
                Debug.LogError("[EmotionClient3D] 마이크 장치 없음");
                continue;
            }

            currentAudioClip = Microphone.Start(micName, false, 2, 16000);
            yield return new WaitForSeconds(2f);
            Microphone.End(micName);

            byte[] audioBytes = SaveWavToBytes(currentAudioClip);

            WWWForm form = new WWWForm();
            form.AddBinaryData("image", imageBytes, "frame.png", "image/png");
            form.AddBinaryData("audio", audioBytes, "audio.wav", "audio/wav");

            UnityWebRequest request = UnityWebRequest.Post(serverUrl, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var json = request.downloadHandler.text;
                EmotionResult result = JsonUtility.FromJson<EmotionResult>(json);
                if (!string.IsNullOrEmpty(result?.face))
                {
                    StartCoroutine(ApplyAndResetExpression(result.face.ToLower()));
                }
            }
            else
            {
                Debug.LogWarning($"[EmotionClient3D] 서버 전송 실패: {request.error}");
            }
        }
    }

    byte[] GetCurrentImagePNG()
    {
        Texture2D tex = new Texture2D(webcamInput.CamTexture.width, webcamInput.CamTexture.height, TextureFormat.RGB24, false);
        tex.SetPixels(webcamInput.CamTexture.GetPixels());
        tex.Apply();
        return tex.EncodeToPNG();
    }

    byte[] SaveWavToBytes(AudioClip clip)
    {
        if (clip == null) return null;

        using (MemoryStream stream = new MemoryStream())
        {
            int samples = clip.samples;
            int channels = clip.channels;
            float[] data = new float[samples * channels];
            clip.GetData(data, 0);

            short[] intData = new short[data.Length];
            byte[] bytesData = new byte[data.Length * 2];

            const int rescaleFactor = 32767;
            for (int i = 0; i < data.Length; i++)
            {
                intData[i] = (short)(data[i] * rescaleFactor);
                byte[] byteArr = System.BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            WriteWavHeader(stream, clip, bytesData.Length);
            stream.Write(bytesData, 0, bytesData.Length);

            return stream.ToArray();
        }
    }

    void WriteWavHeader(Stream stream, AudioClip clip, int dataLength)
    {
        int sampleRate = clip.frequency;
        int channels = clip.channels;
        int byteRate = sampleRate * channels * 2;

        using (BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true))
        {
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + dataLength);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write((short)(channels * 2));
            writer.Write((short)16);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(dataLength);
        }
    }

    IEnumerator ApplyAndResetExpression(string emotion)
    {
        isEmotionPlaying = true;

        // 실시간 눈/입 추적 중단
        if (faceExpressionExtractor != null)
            faceExpressionExtractor.enabled = false;

        // 감정에 따른 BlendShape 적용
        avatarController?.SetEmotion(emotion);


        Debug.Log($"[EmotionClient3D] 감정 적용: {emotion}");

        // 감정 유지 시간
        yield return new WaitForSeconds(expressionDuration);


        // 실시간 눈/입 추적 재개
        yield return new WaitForSeconds(0.1f); // 약간의 딜레이 안정성 보장
        if (faceExpressionExtractor != null)
            faceExpressionExtractor.enabled = true;

        isEmotionPlaying = false;
    }

    [System.Serializable]
    public class EmotionResult
    {
        public string face;
        public string audio;
    }
}
