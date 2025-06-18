using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;
using Unity.VisualScripting;

public class PlayerNetworkBelonging : NetworkBehaviour
{
    [SerializeField] private FirstPersonController controller;
    [SerializeField] private AnimationController animationController;
    [SerializeField] private MessageBubbling messageBubbling;
    [SerializeField] private GameObject buildingPoint;
    [SerializeField] private GameObject camera;
    public NetworkVariable<FixedString128Bytes> Username = new NetworkVariable<FixedString128Bytes>(writePerm: NetworkVariableWritePermission.Server);
    public TMP_Text usernametext;

    void Update()
    {

        if (!IsOwner)
        {
            controller.enabled = false;
            animationController.enabled = false;
            messageBubbling.enabled = false;
            Destroy(camera);
            Destroy(buildingPoint);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SubmitUsernameServerRpc(PlayerPrefs.GetString("Username"));
        }
    }

    [ServerRpc]
    void SubmitUsernameServerRpc(string username)
    {
        Username.Value = username;
    }
}
