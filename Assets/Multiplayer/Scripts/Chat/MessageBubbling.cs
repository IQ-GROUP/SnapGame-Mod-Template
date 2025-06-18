using System;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Netcode.Components;

public class MessageBubbling : NetworkBehaviour
{
    [SerializeField] private GameObject bubbleprefab;

    [ServerRpc(RequireOwnership = false)]
    public void ShowBubbleMessageServerRpc(string message)
    {
        var instance = Instantiate(bubbleprefab, transform.position, transform.rotation);
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn(true);
        instanceNetworkObject.TrySetParent(gameObject.transform);

        ulong objectId = instanceNetworkObject.NetworkObjectId;

        // Now tell clients to set the message
        SetBubbleMessageClientRpc(objectId, message);
    }

    [ClientRpc]
    void SetBubbleMessageClientRpc(ulong objectId, string message)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var obj))
        {
            var bubble = obj.GetComponent<MessageBubble>();
            TMP_Text text = bubble.text;
            SpriteRenderer sprite = bubble.spriteRenderer;

            text.text = WrapText(message, 39);
            sprite.size = CalculateSpriteSize(message);
            obj.transform.position = new Vector3(transform.position.x, CalculateYOffset(message) + transform.position.y, transform.position.z);
        }
    }


    static string WrapText(string text, int lineLength)
    {
        // Truncate the text if it's longer than 200 characters
        if (text.Length > 200)
        {
            text = text.Substring(0, 195) + "...";
        }

        if (string.IsNullOrEmpty(text)) return string.Empty;

        var result = new System.Text.StringBuilder();
        for (int i = 0; i < text.Length; i += lineLength)
        {
            if (i + lineLength > text.Length)
                result.AppendLine(text.Substring(i));
            else
                result.AppendLine(text.Substring(i, lineLength));
        }

        return result.ToString();
    }

    static int CountLines(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;

        // Calculate the number of lines by dividing the length of the text by 39
        return (text.Length + 38) / 39; // Add 38 to ensure rounding up when there are remaining characters
    }

    static Vector2 CalculateSpriteSize(string text)
    {

        int lines = CountLines(text);
        Debug.Log(lines);

        float y = 1;
        if (lines > 1)
        {
            y += 0.2f * (lines - 1);
        }

        float x = 1;
        if (lines > 1)
        {
            x = 2.5f;
        }
        else
        {
            x += 0.05f * (text.Length - 12);
        }

        return new Vector2(x, y);
    }

    static float CalculateYOffset(string text)
    {
        float y = 1.2f;

        y += 0.1f * (CountLines(text) - 1);
        Debug.Log(y);
        return y;
    }

    static float CalculateSpriteYOffset(string text)
    {
        float y = 0;

        y += 0.05f * (CountLines(text) - 1);
        Debug.Log(y);
        return y;
    }
}