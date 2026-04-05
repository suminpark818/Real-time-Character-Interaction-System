using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private GameObject loadingScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }

        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }
    public void Start2DExperience()
    {
        Debug.Log("[SceneLoader] Start2DExperience clicked");
        SceneLoader.Instance.LoadScene("2DAvartar");
    }

    public void Start3DExperience()
    {
        Debug.Log("[SceneLoader] Start3DExperience clicked");
        SceneLoader.Instance.LoadScene("duksae_3d");
    }

}

