using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    bool hasPlayed = false;

    public AITrigger[] triggers;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // V�rifie que c'est bien le joueur
        {
            if (hasPlayed) return; // Emp�che de rejouer si d�j� jou�
            AIVoice.PlaySequentialAudio(triggers);
            hasPlayed = true;
        }
    }
}