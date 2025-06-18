using Unity.Netcode;
using UnityEngine;

public class SpawnAlongNetwork : MonoBehaviour
{
    [SerializeField] private GameObject _object;

    private void Awake()
    {
        _object = gameObject;
    }

    private void Start()
    {
        SpawnObjectAlongNetwork();
    }

    public void SpawnObjectAlongNetwork()
    {
        if (_object != null)
        {
            GetComponent<NetworkObject>().Spawn();
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Object for spawning is not assigned.");
        }
    }
}