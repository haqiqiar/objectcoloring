using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;



public class ObjectColoring : MonoBehaviour
{
    public Renderer rend;
    public Dictionary<string, object> dict = new Dictionary<string, object>();
    public string objectId = "Ob001";
    public string selectedType = "Temperature";
    public TextMesh t;

    public void FetchData()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://iotunity.firebaseio.com/");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        FirebaseDatabase.DefaultInstance
            .GetReference("Sensors")
            .OrderByChild("Objects/" + objectId).EqualTo("true")
            .GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    // Handle the error...
                    Debug.Log("handle Error");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    // Debug.Log(snapshot.GetRawJsonValue());
                    foreach (DataSnapshot sensor in snapshot.Children)
                    {
                        var values = sensor.Child("Values").Value as Dictionary<string, object>;
                        foreach (var item in values)
                        {
                            dict.Add(item.Key, item.Value);
                        }
                    }
                }
            });
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
        FetchData();
    }

    // Update is called once per frame
    void Update()
    {
        rend.material.color = GetColor();
    }
}
