using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class ObjectColoring : MonoBehaviour
{
    public Renderer rend;
    public Dictionary<string, object> dict = new Dictionary<string, object>();
    public string objectId = "Ob001";
    public string selectedType = "Temperature";
    public TextMesh t;

    public WWW www = null;

    public void FetchData()
    {

        var url = "https://iotunity.firebaseio.com/Sensors.json";
        Debug.Log("testi");
        www = new WWW(url);
        Debug.Log("test");

        foreach (DataSnapshot sensor in snapshot.Children)
        {
            var values = sensor.Child("Values").Value as Dictionary<string, object>;
            foreach (var item in values)
            {
                dict.Add(item.Key, item.Value);
            }
        }

    }

    public class SensorData
    {
        public string SensorID;
    }

    private IEnumerator ReceiveResponse()
    {
        yield return www;
        Debug.Log("testaaa");
        var a   = JsonUtility.FromJson<object[]>(www.text);

        Debug.Log(a[0] as );


    }

    public Color GetColor()
    {
        var res = rend.material.color;
        object data = null;
        switch (selectedType)
        {
            case "Humidity":
                if (dict.TryGetValue("Humidity", out data))
                {
                    Dictionary<string, object> value = data as Dictionary<string, object>;
                    var hum = float.Parse(value["Value"].ToString());
                    t.text = value["Value"].ToString() + " " + value["Unit"];
                    res = ValueCalculator.tempToColor(hum);
                }
                break;
            case "Temperature":
                if (dict.TryGetValue("Temperature", out data))
                {
                    Dictionary<string, object> value = data as Dictionary<string, object>;
                    var temp = float.Parse(value["Value"].ToString());
                    t.text = value["Value"].ToString() + "°" + value["Unit"];
                    res = ValueCalculator.tempToColor(temp);
                }
                break;
            case "Noise":
                if (dict.TryGetValue("Noise", out data))
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
        Debug.Log("lol");
        FetchData();
    }

    // Update is called once per frame
    void Update()
    {
        rend.material.color = GetColor();
    }
}
