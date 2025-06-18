using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class SyncSkinnedMeshMaterial : NetworkBehaviour
{
    [SerializeField] private SkinnedMeshRenderer playerMeshRenderer;

    // Must use shared material property name from Shader Graph (e.g., "_BaseColor")
    [SerializeField] private string materialColorProperty = "_BaseColor";

    // Syncs HEX color string
    private NetworkVariable<FixedString64Bytes> syncedHexColor = new NetworkVariable<FixedString64Bytes>(
        "#808080", // Default: grey
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Owner sets initial color from PlayerPrefs
        if (IsOwner)
        {
            string hexColor = PlayerPrefs.GetString("Color", "#808080"); // Default grey
            if (ColorUtility.TryParseHtmlString(hexColor, out Color _))
            {
                syncedHexColor.Value = hexColor;
            }
            else
            {
                Debug.LogWarning($"Invalid hex in PlayerPrefs: {hexColor}, defaulting to grey.");
                syncedHexColor.Value = "#808080";
            }
        }

        // Apply initial color
        ApplyHexColor(syncedHexColor.Value.ToString());

        // Subscribe to changes
        syncedHexColor.OnValueChanged += OnHexColorChanged;
    }

    public override void OnNetworkDespawn()
    {
        syncedHexColor.OnValueChanged -= OnHexColorChanged;
        base.OnNetworkDespawn();
    }

    private void OnHexColorChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        ApplyHexColor(newValue.ToString());
    }

    private void ApplyHexColor(string hex)
    {
        if (playerMeshRenderer == null || string.IsNullOrEmpty(hex))
            return;

        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            playerMeshRenderer.material.SetColor(materialColorProperty, color);
        }
        else
        {
            Debug.LogError($"Failed to parse HEX color: {hex}");
        }
    }
}