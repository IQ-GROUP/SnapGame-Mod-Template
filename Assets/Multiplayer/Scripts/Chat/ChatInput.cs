using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ChatInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    private MessageBubbling messageBubbling;

    void Update(){

        foreach(MessageBubbling i in FindObjectsOfType<MessageBubbling>()){
            if(i.enabled){
                messageBubbling = i;
            }
        }

        if(Input.GetKeyDown(KeyCode.Return)){
            messageBubbling.ShowBubbleMessageServerRpc(inputField.text);
            inputField.text = "";
        }
    }
}