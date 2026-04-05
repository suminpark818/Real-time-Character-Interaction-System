using UnityEngine;
using TMPro;

public class EmotionDisplayUI : MonoBehaviour
{
    public TextMeshProUGUI finalEmotionText;

    public void UpdateFinalEmotion(string emotion)
    {
        finalEmotionText.text = $"Emotion: {emotion}";
    }
}
