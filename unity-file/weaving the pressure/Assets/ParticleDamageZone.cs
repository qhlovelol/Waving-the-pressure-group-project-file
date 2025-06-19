using UnityEngine;

public class ParticleDamageZone2D : MonoBehaviour
{
    public int damagePerHit = 1;
    private GameHealthManager health;

    void Start()
    {
        health = FindAnyObjectByType<GameHealthManager>();
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("💢 Particle collided with zone: " + other.name);
        if (health != null)
        {
            health.TakeDamage(damagePerHit);
        }
    }
}
