using UnityEngine;
using UnityEngine.SceneManagement;

public class AvatarManager : MonoBehaviour
{
    public string avatarName; // ¿¹: "hoshinonya"

    public void OnSelect()
    {
        SelectedAvatar.AvatarName = avatarName;
        SceneManager.LoadScene("Main2DScene");
    }
}
