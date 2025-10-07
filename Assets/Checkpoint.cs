using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player != null)
        {
            player.SetCheckpoint(transform.position);
            Debug.Log("Checkpoint atteint : " + name);
        }
    }
}