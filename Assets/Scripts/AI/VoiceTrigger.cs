using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    bool hasPlayed = false;

    public AITrigger[] triggers;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Vérifie que c'est bien le joueur
        {
            if (hasPlayed) return; // Empêche de rejouer si déjà joué
            AIVoice.PlaySequentialAudio(triggers);
            hasPlayed = true;
        }
    }
}