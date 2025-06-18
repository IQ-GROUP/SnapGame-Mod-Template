using Unity.Netcode;
using UnityEngine;

public class SyncSpriteRendererSizeNetwork : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Network variables to store the width and height of the sprite
    private NetworkVariable<Vector2> syncSize = new NetworkVariable<Vector2>(
        new Vector2(1f, 1f), 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Initialize with the actual sprite size
            syncSize.Value = spriteRenderer.size;
        }

        // Subscribe to network variable change
        syncSize.OnValueChanged += OnSizeChanged;

        // Apply the correct size for new clients
        UpdateSpriteSize(syncSize.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetSpriteSizeServerRpc(Vector2 newSize)
    {
        syncSize.Value = newSize;
    }

    private void OnSizeChanged(Vector2 previousSize, Vector2 newSize)
    {
        UpdateSpriteSize(newSize);
    }

    private void UpdateSpriteSize(Vector2 newSize)
    {
        spriteRenderer.size = newSize;
    }

    public override void OnDestroy()
    {
        syncSize.OnValueChanged -= OnSizeChanged;
        base.OnDestroy();
    }
}
