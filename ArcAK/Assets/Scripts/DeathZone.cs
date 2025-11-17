using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Ball"))
    {
        var gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.GameOver("Ball Lost"); // или gm.Restart();
        Destroy(other.gameObject);
    }
}

}