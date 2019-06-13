using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using MiniJSON;

public class ObjectColoring : MonoBehaviour
{
    public Renderer rend;
    public Dictionary<string, object> valueDict = new Dictionary<string, object>();
    public string objectId = "Ob001";
    public string selectedType = "Humidity";
    public TextMesh t;

    #region data and firebase

    IEnumerator FetchObjectSensors()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://iotunity.firebaseio.com/Objects/" + objectId + ".json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            var dictValue = Json.Deserialize(www.downloadHandler.text) as Dictionary<string, object>;

            foreach (var item in dictValue)
            {
                Debug.Log(item.Key);
                StartCoroutine(FetchSensorData(item.Key));
            }
        }
    }

    IEnumerator FetchSensorData(string key)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://iotunity.firebaseio.com/Sensors/" + key + "/Values.json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            var dictValue = Json.Deserialize(www.downloadHandler.text) as Dictionary<string, object>;
            foreach (var item in dictValue)
            {
                Debug.Log(item.Key);
                if (!valueDict.ContainsKey(item.Key))
                {
                    valueDict.Add(item.Key, dictValue[item.Key]);
                }
            }
        }
    }

    #endregion

    public Color GetColor()
    {
        var res = rend.material.color;
        object data = null;
        switch (selectedType)
        {
            case "Humidity":
                if (valueDict.TryGetValue("Humidity", out data))
                {
                    Dictionary<string, object> value = data as Dictionary<string, object>;
                    var hum = float.Parse(value["Value"].ToString());
                    t.text = value["Value"].ToString() + " " + value["Unit"];
                    res = ValueCalculator.tempToColor(hum);
                }
                break;
            case "Temperature":
                if (valueDict.TryGetValue("Temperature", out data))
                {
                    Dictionary<string, object> value = data as Dictionary<string, object>;
                    var temp = float.Parse(value["Value"].ToString());
                    t.text = value["Value"].ToString() + "°" + value["Unit"];
                    res = ValueCalculator.tempToColor(temp);
                }
                break;
            case "Noise":
                if (valueDict.TryGetValue("Noise", out data))
                {
                    Dictionary<string, object> value = data as Dictionary<string, object>;
                    var noise = float.Parse(value["Value"].ToString());
                    t.text = value["Value"].ToString() + " " + value["Unit"];
                    res = ValueCalculator.tempToColor(noise);
                }
                break;
        }

        return res;
    }

    void Start()
    {
        StartCoroutine(FetchObjectSensors());

        rend = GetComponent<Renderer>();
        rend.material.color = Color.white;
        GameObject text = new GameObject();
        t = text.AddComponent<TextMesh>();
        t.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        t.fontSize = 150;
        t.anchor = TextAnchor.MiddleCenter;
        t.color = Color.black;
        t.transform.position = gameObject.transform.position;

        t.transform.position += new Vector3(0f, 0f, -1f);

    }

    // Update is called once per frame
    void Update()
    {
        rend.material.color = GetColor();
    }
}
