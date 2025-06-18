using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    [SerializeField] private Camera gamePreviewCamera;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private TMP_InputField ipAddressField;

    private void Awake()
    {
        ipAddressField.text = "127.0.0.1";

        serverButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            OnNetworkStarted();
        });

        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            OnNetworkStarted();
        });

        clientButton.onClick.AddListener(() => {
            JoinLAN(ipAddressField.text.Trim());
        });
    }

    private void OnNetworkStarted()
    {
        gamePreviewCamera.enabled = false;
        gameUI.SetActive(true);
        gameObject.SetActive(false);
    }

    private void JoinLAN(string ipAddress)
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ipAddress, 7777); // Default UTP port (can be customized)

        NetworkManager.Singleton.StartClient();
        OnNetworkStarted();
    }
}