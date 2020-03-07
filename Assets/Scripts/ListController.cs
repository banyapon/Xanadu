using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
public class ListController : MonoBehaviour
{

    public GameObject ContentPanel;
    public GameObject ListItemPrefab;
    [TextArea]
    public string jsonData;
    [TextArea]
    public string url;
    public int page = 1;
    ArrayList TempleArray;
    void Start()
    {
        //JSON
        UnityWebRequest www = new UnityWebRequest(url);
        StartCoroutine(FetchData(url));
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
            for (int i = 0; i < jsonNode["response"]["categories"].Count; i++)
            {

                GameObject newtemple = Instantiate(ListItemPrefab) as GameObject;
                newtemple.gameObject.name = jsonNode["response"]["categories"][i]["slug"];
                ListItemController controller = newtemple.GetComponent<ListItemController>();
                newtemple.transform.parent = ContentPanel.transform;
                newtemple.transform.localScale = Vector3.one;
                controller.Name.text = jsonNode["response"]["categories"][i]["name"].ToString();
                string getJSONImg = jsonNode["response"]["categories"][i]["cover_url"].ToString();
                string replaceQuote = getJSONImg.Replace("\"", "");
                string urlImg = replaceQuote.Replace("\\", "");

                //Debug.Log("urlImg: "+urlImg);

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