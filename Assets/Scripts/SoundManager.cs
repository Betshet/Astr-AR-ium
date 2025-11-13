using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public bool loop = false;
    }

    [Header("Liste des sons disponibles")]
    public List<Sound> sounds = new List<Sound>();

    private Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Crée un AudioSource pour chaque son
        foreach (Sound s in sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = s.clip;
            source.volume = s.volume;
            source.loop = s.loop;
            audioSources[s.name] = source;
        }
    }

    public void Play(string soundName, bool loop = false)
    {
        if (audioSources.ContainsKey(soundName))
        {
            AudioSource source = audioSources[soundName];
            source.loop = loop; // change la boucle si besoin
            source.Play();
        }
        else
        {
            Debug.LogWarning("SoundManager: son introuvable -> " + soundName);
        }
    }

    public void Stop(string soundName)
    {
        if (audioSources.ContainsKey(soundName))
            audioSources[soundName].Stop();
    }

    public bool IsPlaying(string soundName)
    {
        return audioSources.ContainsKey(soundName) && audioSources[soundName].isPlaying;
    }
}
