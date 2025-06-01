using UnityEngine;
using UnityEngine.Audio;

namespace Utils
{
    public class AudioPlayer : MonoBehaviour
    {
        private static AudioPlayer _instance;

        [SerializeField] private AudioSource source;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static void PlaySound(AudioResource resource)
        {
            _instance.source.resource = resource;
            _instance.source.Play();
        }
    }
}