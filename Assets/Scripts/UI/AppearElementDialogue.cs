using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearElementDialogue : MonoBehaviour
{
    public GameObject elementDialogue;
    // Start is called before the first frame update
    void Start()
    {

        // elementDialogue = Find("ElementDialogue");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetTrue()
    {
        elementDialogue.SetActive(true);
    }
}
