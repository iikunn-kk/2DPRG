using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }
    void OnEnable()
    {
        EventCenter.Instance.AddEventListener<int>("Hello EventCenter", (eventcenter) =>
        {
            Debug.Log("Hello EventCenter" + eventcenter);
        });


        EventCenter.Instance.AddEventListener("Hello EventCenter2", PrintHelloEventer);
    }

    public void PrintHelloEventer()
    {
        Debug.Log("HelloWordl EventCenterqweqweqw");
    }
    void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<int>("Hello EventCenter", (eventcenter) =>
        {
            Debug.Log("Hello EventCenter" + eventcenter);
        });

        EventCenter.Instance.RemoveEventListener("Hello EventCenter2", PrintHelloEventer);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
