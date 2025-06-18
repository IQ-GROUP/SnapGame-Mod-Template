using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Collections;

public class SyncTextNetwork : NetworkBehaviour
{
    public TextMeshPro textMeshPro;
    private NetworkVariable<FixedString128Bytes> syncedText = new NetworkVariable<FixedString128Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Start()
    {
        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshPro component not assigned.");
            return;
        }
    }

    public override void OnNetworkSpawn()
    {
        syncedText.OnValueChanged += OnTextChanged;

        if (string.IsNullOrEmpty(textMeshPro.text) && !string.IsNullOrEmpty(syncedText.Value.ToString()))
        {
            textMeshPro.text = syncedText.Value.ToString();
        }
        else if (IsOwner && !string.IsNullOrEmpty(textMeshPro.text))
        {
            RequestTextUpdateServerRpc(textMeshPro.text);
        }
    }

    private void OnTextChanged(FixedString128Bytes previous, FixedString128Bytes current)
    {
        if (string.IsNullOrEmpty(textMeshPro.text))
        {
            textMeshPro.text = current.ToString();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestTextUpdateServerRpc(string newText, ServerRpcParams rpcParams = default)
    {
        if (!string.IsNullOrEmpty(newText))
        {
            syncedText.Value = newText;
        }
    }
}