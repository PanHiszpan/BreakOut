using UnityEngine.Audio;
using UnityEngine;
using System;


/// <summary>
/// prefab AudioMenagera wrzucic na kazda scene
/// dzwieki dodac do listy w obiekcie, 
/// odpalac tworzac referencje do AudioMenagera jak w Sword.cs, podpinajac w inspektorze w Unity i odpalac wywolujac funkcje Play("[nazwa dzwięku z listy]")
/// </summary>
public class AudioMenager : MonoBehaviour
{
    public static AudioMenager instance;

    public Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        // ----- dodaje Audiosource'y z ustawieniami z listy do obiektu AudioMenagera--------
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Nie znalazlem pliku audio: " + name);
            return;
        }
        s.source.Play();
    }
}
