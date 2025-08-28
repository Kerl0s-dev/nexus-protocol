using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Vérifie que c'est bien le joueur
        {
            if (hasPlayed) return; // Empêche de rejouer si déjà joué
            AIVoice.PlaySequentialAudio(AITrigger.Welcome, AITrigger.Rules, AITrigger.Advance);
            hasPlayed = true;
        }
    }
}