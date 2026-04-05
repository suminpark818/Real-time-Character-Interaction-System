using UnityEngine;

public class LightingManager : MonoBehaviour
{
    [System.Serializable]
    public class LightSetting
    {
        public Color color;
        public float intensity;
        public Vector3 rotation;
    }

    public Light directionalLight;
    public LightSetting[] presets;
    private int currentIndex = 0;

    public void ChangeLighting()
    {
        if (presets.Length == 0 || directionalLight == null) return;

        currentIndex = (currentIndex + 1) % presets.Length;

        LightSetting setting = presets[currentIndex];
        directionalLight.color = setting.color;
        directionalLight.intensity = setting.intensity;
        directionalLight.transform.eulerAngles = setting.rotation;

        Debug.Log($"💡 Lighting changed to: {currentIndex}");
    }
}
