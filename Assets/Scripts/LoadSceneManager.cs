using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{   
    public Text loadText;
    public string sceneName, param;
    public float progress;
    void Start(){
        param = PlayerPrefs.GetString("param");
        LoadScene(sceneName);
    }

    void Update(){
        loadText.text = (progress * 100).ToString("n0") + "%";
    }

    public void LoadScene(string sceneName)
    {
        PlayerPrefs.SetString("subparam", param);
        StartCoroutine(LoadSceneObject(sceneName));
    }

    public IEnumerator LoadSceneObject(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            progress = Mathf.Clamp01(async.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100).ToString("n0") + "%");

            if (progress == 1f)
            {
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}