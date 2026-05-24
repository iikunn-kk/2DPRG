using System.Collections;
using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    DialogueTreeController dialogueTree;
    private void Start()
    {
        dialogueTree = GetComponent<DialogueTreeController>();
    }
    public void Talk()
    {
        dialogueTree.StartDialogue();
    }
}
