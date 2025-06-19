using UnityEngine;

public class PixelDroplet : MonoBehaviour
{
    public int damage = 100;
    public GameObject destroyEffect; // ✅ 独立的爆炸特效 prefab

    void Start()
    {
        GetComponent<Rigidbody2D>().freezeRotation = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActiveAndEnabled) return;

        if (other.CompareTag("DamageZone"))
        {
            var health = FindAnyObjectByType<GameHealthManager>();
            if (health != null)
                health.TakeDamage(damage);
        }

        // ✅ 生成销毁特效（确保不是当前物体的子物体）
        if (destroyEffect != null)
        {
            GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
            // 可选：手动销毁（保险起见）
            Destroy(effect, 1.5f);
        }

        Destroy(gameObject);
    }
}
