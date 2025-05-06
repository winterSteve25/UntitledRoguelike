using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class StartGameScript : MonoBehaviour
    {
        private void Start()
        {
            NetworkManager.Singleton.OnConnectionEvent += OnConnection;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnConnectionEvent -= OnConnection;
        }

        private void OnConnection(NetworkManager manager, ConnectionEventData data)
        {
            if (data.EventType != ConnectionEvent.ClientConnected) return;
            if (!manager.IsServer || manager.ConnectedClients.Count != 2) return;
            manager.SceneManager.LoadScene("Battle Level", LoadSceneMode.Single);
            Debug.Log("Transitioning!");
        }
    }
}