using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // V�rifie que c'est bien le joueur
        {
            if (hasPlayed) return; // Emp�che de rejouer si d�j� jou�
            AIVoice.PlaySequentialAudio(AITrigger.Welcome, AITrigger.Rules, AITrigger.Advance);
            hasPlayed = true;
        }
    }
}