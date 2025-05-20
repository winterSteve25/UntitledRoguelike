using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class InCombatDisconnect : MonoBehaviour
    {
        private void OnEnable()
        {
            NetworkManager.Singleton.OnConnectionEvent += SingletonOnOnConnectionEvent;
        }
        
        private void OnDisable()
        {
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnConnectionEvent -= SingletonOnOnConnectionEvent;
        }
        
        private void SingletonOnOnConnectionEvent(NetworkManager networkManager, ConnectionEventData data)
        {
            if (data.EventType is not (ConnectionEvent.ClientDisconnected or ConnectionEvent.PeerDisconnected)) return;
            networkManager.Shutdown();
            SceneManager.LoadScene("Connect", LoadSceneMode.Single);
        }
    }
}