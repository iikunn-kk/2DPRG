using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test2 : MonoBehaviour
{
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            Debug.Log("Click Button");
            EventCenter.Instance.EventTrigger<int>("Hello EventCenter", 100);


            EventCenter.Instance.EventTrigger("Hello EventCenter2");
        });
    }


    // Update is called once per frame
    void Update()
    {

    }
}
