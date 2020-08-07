using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    #region Singleton
    public static NetworkManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }
    #endregion

    public GameObject playerPrefab;
    public GameObject othersPlayerPrefab;

    void Start()
    {
        DontDestroyOnLoad(this);
        UnityThread.initUnityThread();
    }

    public void Connect (string ip, int port)
    {
        Debug.Log("Connecting to server, please wait...");
        DataReceiver.Initialize();
        Client.Initialize(ip, port);
    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }

    #region Useful methods
    public void CreateLocalPlayer ()
    {
        Vector3 spawnPoint = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f);
        GameObject localPlayer = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
        NetworkIdentifier identifier = localPlayer.GetComponent<NetworkIdentifier>();
        identifier.id = Client.connectionID;
        identifier.isMine = true;

        DataSender.SendInstantiate(spawnPoint, "others");
    }

    public void CreateNetworkPlayer (int id, Vector3 spawnPos)
    {
        GameObject networkPlayer = Instantiate(othersPlayerPrefab, spawnPos, Quaternion.identity);
        networkPlayer.GetComponent<NetworkIdentifier>().id = id;
    }

    public void UpdateNetworkPlayerTransform(int targetID, Vector3 newPos, float newZRot)
    {
        GameObject targetPlayer = GetNetworkPlayer(targetID);
        if (targetPlayer != null)
        {
            targetPlayer.transform.position = newPos;
            targetPlayer.transform.eulerAngles = new Vector3(targetPlayer.transform.eulerAngles.x, targetPlayer.transform.eulerAngles.y, newZRot);
        }
    }

    public static GameObject GetLocalPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player.GetComponent<NetworkIdentifier>().isMine)
        {
            return player;
        }
        return null;
    }

    public static GameObject GetNetworkPlayer (int id)
    {
        NetworkIdentifier[] networkPlayers = FindObjectsOfType<NetworkIdentifier>();
        foreach (NetworkIdentifier networkPlayer in networkPlayers)
        {
            if(networkPlayer.id == id)
            {
                return networkPlayer.gameObject;
            }
        }
        return null;
    }

    public static void DestroyGameObject (GameObject go)
    {
        Destroy(go);
    }
    #endregion
}
