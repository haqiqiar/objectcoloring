using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;



public class ObjectColoring : MonoBehaviour
{

    public Color altColor = Color.blue;
    public Renderer rend;
    public float temp;
    
    void Start()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://iotunity.firebaseio.com/");

        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        temp = 0; 
        rend = GetComponent<Renderer>();
        rend.material.color = altColor;

        FirebaseDatabase.DefaultInstance
            .GetReference("Sensors")
            .OrderByChild("Objects/Ob001").EqualTo("true")
            .GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    // Handle the error...
                    Debug.Log("handle Error");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    // Do something with snapshot...
                    Debug.Log(snapshot.GetRawJsonValue());
                    foreach(DataSnapshot item in snapshot.Children)
                    {
                        Debug.Log(item.);
                        item.GetValue(true)
                    }
                }
            });

    }

    // Update is called once per frame
    void Update()
    {   
        temp += 0.5f;

        rend.material.color = ValueCalculator.tempToColor(temp);
    }
}
