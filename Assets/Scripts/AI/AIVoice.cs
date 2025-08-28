using System.Collections;
using UnityEngine;

public enum AITrigger
{
    Welcome,
    Rules,
    Advance,
}

public class AIVoice : MonoBehaviour
{
    [System.Serializable]
    public struct AILine
    {
        public string name; // ex: "Death Line"
        public AITrigger trigger; // ex: "death", "dash", "idle"
        public AudioClip line; // plusieurs répliques possibles
    }

    public static AIVoice instance;

    public AILine[] lines;
    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlayLine(AITrigger trigger)
    {
        foreach (var line in instance.lines)
        {
            if (line.trigger == trigger)
            {
                AudioClip chosenLine = line.line;
                Debug.Log("AI says: " + chosenLine);
                // Ici tu peux soit TTS, soit Audio pre-record
                instance.audioSource.PlayOneShot(chosenLine);
                break;
            }
        }
    }

    public static void PlaySequentialAudio(params AITrigger[] triggers)
    {
        instance.StartCoroutine(instance.PlaySequenceCoroutine(triggers));
    }

    private IEnumerator PlaySequenceCoroutine(AITrigger[] triggers)
    {
        foreach (var trigger in triggers)
        {
            AudioClip clip = null;

            foreach (var line in lines)
            {
                if (line.trigger == trigger)
                {
                    clip = line.line;
                    break;
                }
            }

            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
                Debug.Log("AI sequential says: " + clip.name);

                yield return new WaitForSeconds(clip.length);
            }
        }
    }
}
