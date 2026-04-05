using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AvatarLoader : MonoBehaviour
{
    [SerializeField] private Canvas targetCanvas;

    void Start()
    {
#if UNITY_EDITOR
        string avatarName = SelectedAvatar.AvatarName;

        if (!string.IsNullOrEmpty(avatarName))
        {
            string path1 = $"Assets/2DAvartar/{avatarName}/{avatarName}.prefab";
            string path2 = $"Assets/2DAvartar/{avatarName}.prefab";
            string path3 = $"Assets/2DAvartar/{avatarName}/{avatarName}/{avatarName}.prefab";

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path1);

            if (prefab == null)
            {
                Debug.LogWarning($"⚠️ 첫 번째 경로에서 프리팹을 찾을 수 없음: {path1}");
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path2);
            }

            if (prefab == null)
            {
                Debug.LogWarning($"⚠️ 두 번째 경로에서도 프리팹을 찾을 수 없음: {path2}");
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path3);
            }

            if (prefab == null)
            {
                Debug.LogError($"❌ 세 번째 경로에서도 프리팹을 찾을 수 없습니다: {path3}");
                return;
            }

            GameObject avatarInstance = Instantiate(prefab, targetCanvas.transform);
            avatarInstance.name = "Live2DAvatar_" + avatarName;

            avatarInstance.transform.localScale = new Vector3(5f, 5f, 1f);

            RectTransform rectTransform = avatarInstance.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.localScale = Vector3.one * 3f;
            }

            if (avatarInstance.GetComponent<SetupLive2D>() == null)
            {
                avatarInstance.AddComponent<SetupLive2D>();
                Debug.Log("✅ SetupLive2D 스크립트가 자동으로 추가되었습니다.");
            }
        }
#endif
    }
}
