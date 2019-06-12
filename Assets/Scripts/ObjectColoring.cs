using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;

public class ObjectColoring : MonoBehaviour
{
    public Renderer rend;
    public Dictionary<string, object> valueDict = new Dictionary<string, object>();
    public string objectId = "Ob001";
    public string selectedType = "Humidity";
    public TextMesh t;
    public Firebase firebase;

    #region data and firebase
    public void InitFirebase()
    {
        this.firebase = Firebase.CreateNew("https://iotunity.firebaseio.com/");
        // firebase.OnGetSuccess += GetOKHandler;
        firebase.OnGetFailed += GetFailHandler;
    }

    public void GetObjectSensors()
    {
        Firebase sensors = firebase.Child("Objects/" + objectId, true);
        sensors.OnGetSuccess += GetObjectSensorsHandler;
        sensors.GetValue();
    }

    void GetObjectSensorsHandler(Firebase sender, DataSnapshot snapshot)
    {
        List<string> keys = snapshot.Keys;
        if (keys != null)
            foreach (string key in keys)
            {
                GetSensorData(key);
            }
    }

    public void GetSensorData(string key)
    {
        Firebase sensor = firebase.Child("Sensors/" + key + "/Values", true);
        sensor.OnGetSuccess += GetSensorDataHandler;
        sensor.GetValue();
    }

    void GetSensorDataHandler(Firebase sender, DataSnapshot snapshot)
    {
        Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
        List<string> keys = snapshot.Keys;

        if (keys != null)
            foreach (string key in keys)
            {
                if (!valueDict.ContainsKey(key))
                {
                    valueDict.Add(key, dict[key]);
                } 
            }
    }

    public void FetchData()
    {
        GetObjectSensors();
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
        InitFirebase();

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
        
        FetchData();
    }

    // Update is called once per frame
    void Update()
    {
        rend.material.color = GetColor();
    }

    #region handler
    void GetOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        Debug.Log("[OK] Get from key: <" + sender.FullKey + ">");
        Debug.Log("[OK] Raw Json: " + snapshot.RawJson);

        Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
        List<string> keys = snapshot.Keys;

        if (keys != null)
            foreach (string key in keys)
            {
                Debug.Log(key + " = " + dict[key].ToString());
            }
    }

    void GetFailHandler(Firebase sender, FirebaseError err)
    {
        Debug.Log("[ERR] Get from key: <" + sender.FullKey + ">,  " + err.Message + " (" + (int)err.Status + ")");
    }

    #endregion
}
