using UnityEngine;

public class SimpleBrick : MonoBehaviour
{
    public int hp = 1;

    public void TakeDamage(int dmg = 1)
    {
        hp -= dmg;
        if (hp <= 0) DestroyBrick();
    }

    void DestroyBrick()
    {
        var gm = FindObjectOfType<GameManager>();
        if (gm != null) gm.OnBrickDestroyed();
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        var ball = col.collider.GetComponent<BallController>();
        if (ball != null) TakeDamage(1);
    }
}