using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.SceneManagement;
public class CollectionMode : MonoBehaviour
{
    public GameObject ContentPanel,NextButton,PrevButtn;
    public GameObject ListItemPrefab;
    [TextArea]
    public string jsonData;
    [TextArea]
    public string url,param;
    public int page,count_page;
    public string full_url;
    ArrayList TempleArray;
    public Text LogoWord;
    void Start()
    {
        page = PlayerPrefs.GetInt("page",0);
        if(page > 1){
            NextButton.SetActive(true);
            PrevButtn.SetActive(true);
        }else{
            NextButton.SetActive(true);
            PrevButtn.SetActive(false);
        }

        if(page <= 1){
            page =1;
        }
        //JSON
        param = PlayerPrefs.GetString("param");
        
        if(page == null || page == 0){
            page =1;
        }
        LogoWord.text = "ザナドゥ:"+param;
        full_url = url + "/"+param+"/"+page.ToString();
        UnityWebRequest www = new UnityWebRequest(full_url);
        StartCoroutine(FetchData(full_url));
    }

    void Update(){
        
    }

    public void GetNext(){
        page = page + 1;
        PlayerPrefs.SetInt("page",page);
        LoadScene("Collection");
    }

    public void GetPrev(){
        page = page - 1;
        PlayerPrefs.SetInt("page",page);
        LoadScene("Collection");
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneObject(sceneName));
    }

    public IEnumerator LoadSceneObject(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            float progress = Mathf.Clamp01(async.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100).ToString("n0") + "%");

            if (progress == 1f)
            {
                async.allowSceneActivation = true;
            }
            yield return null;
        }
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
            for (int i = 0; i < jsonNode["response"]["videos"].Count; i++)
            {

                GameObject newtemple = Instantiate(ListItemPrefab) as GameObject;
                newtemple.gameObject.name = jsonNode["response"]["videos"][i]["vid"];
                ListItemController controller = newtemple.GetComponent<ListItemController>();
                newtemple.transform.parent = ContentPanel.transform;
                newtemple.transform.localScale = Vector3.one;
                controller.Name.text = jsonNode["response"]["videos"][i]["title"].ToString();
                string getJSONImg = jsonNode["response"]["videos"][i]["preview_url"].ToString();
                string replaceQuote = getJSONImg.Replace("\"", "");
                string urlImg = replaceQuote.Replace("\\", "");
                int total_videos = int.Parse(jsonNode["response"]["total_videos"]);
                
                count_page = total_videos / 50;
                Debug.Log("total_videos: "+count_page);
                

                UnityWebRequest wwwTexture = UnityWebRequestTexture.GetTexture(urlImg);
                yield return wwwTexture.SendWebRequest();

                if (wwwTexture.isNetworkError || wwwTexture.isHttpError)
                {
                    Debug.Log(wwwTexture.error);
                }
                else
                {
                    Texture2D myTexture = ((DownloadHandlerTexture)wwwTexture.downloadHandler).texture;
                    controller.Icon.sprite = Sprite.Create(myTexture,
                    new Rect(0, 0, myTexture.width, myTexture.height),
                    new Vector2(0, 0));
                }
                

            }
        }
    }
}
