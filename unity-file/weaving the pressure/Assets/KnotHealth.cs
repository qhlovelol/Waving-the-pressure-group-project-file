using UnityEngine;

public class KnotHealth : MonoBehaviour
{
    public int maxHits = 500;
    private int currentHits;
    private Vector3 originalPos;
    private bool isShaking = false;

    void Start()
    {
        currentHits = maxHits;
        originalPos = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是不是粒子（你给粒子 prefab 加了 tag "Droplet" 吗？）
        if (other.CompareTag("Particle"))
        {
            currentHits--;
            Debug.Log($"💥 Knot hit! Remaining health: {currentHits}");

            if (!isShaking) StartCoroutine(Shake());

            Destroy(other.gameObject); // ✅ 粒子触碰后销毁

            if (currentHits <= 0)
            {
                Debug.Log("❌ Knot destroyed.");
                Destroy(gameObject);
            }
        }
    }

    System.Collections.IEnumerator Shake()
    {
        isShaking = true;
        float duration = 0.1f;
        float strength = 0.05f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = originalPos + (Vector3)(Random.insideUnitCircle * strength);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
        isShaking = false;
    }
}
