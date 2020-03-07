using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using SimpleJSON;

public class ContentModule : MonoBehaviour
{
    public string sceneName, param,subparam;
    public GameObject ContentPanel;
    public VideoPlayer videoPlayer;
    public GameObject ListItemPrefab;
    public Text Title,Keyword;
    [TextArea]
    public string jsonData;
    [TextArea]
    public string url;
    public string full_url;
    ArrayList TempleArray;


    UniWebView webView;
    GameObject webViewGameObject;
    public Text text;
    public string emebede_url;

    void Start()
    {
        param = PlayerPrefs.GetString("param");
        subparam = PlayerPrefs.GetString("subparam");
        full_url = ""+url+"/"+subparam;
        UnityWebRequest www = new UnityWebRequest(full_url);
        StartCoroutine(FetchData(full_url));
    }

    public void createWebView()
    {
        webViewGameObject = new GameObject("UniWebView");
        webView = webViewGameObject.AddComponent<UniWebView>();
        webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        webView.Load(emebede_url);
        webView.OnShouldClose += (view) => {
            webView = null;
            return true;
        };

        webView.OnMessageReceived += (view, message) =>
        {
            if (message.Path.Equals("view-closed"))
            {
                string msg = message.Args["message"];
                Debug.Log("Your message is: " + msg);
                text.text = msg;

            }
        };
        Debug.Log("Button clicked");
        webView.Show();
    }

    public void BackMenu(){
        PlayerPrefs.SetString("param","");
        PlayerPrefs.SetString("subparam","");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    IEnumerator FetchData(string URL)
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            jsonData = www.downloadHandler.text;
            JSONNode jsonNode = SimpleJSON.JSON.Parse(jsonData);
            Debug.Log("VID: "+jsonNode["response"]["video"]["vid"]);
            Debug.Log("title: "+jsonNode["response"]["video"]["title"]);
            Debug.Log("preview_video_url: "+jsonNode["response"]["video"]["preview_video_url"]);
            Debug.Log("embedded_url: "+jsonNode["response"]["video"]["embedded_url"]);
            Title.text = ""+jsonNode["response"]["video"]["title"];
            Keyword.text = ""+jsonNode["response"]["video"]["keyword"];
            string sample_mp4 = jsonNode["response"]["video"]["preview_video_url"];
            emebede_url = jsonNode["response"]["video"]["embedded_url"];
            StartCoroutine(PlayVideoSample(sample_mp4));
            
        }
    }

    IEnumerator PlayVideoSample(string videoSample)
    {
        yield return new WaitForSeconds(1.5f);
        videoPlayer.source = VideoSource.Url;
        string newUrl = videoSample;
        Debug.Log(newUrl);
        videoPlayer.url = newUrl;
    }
}
