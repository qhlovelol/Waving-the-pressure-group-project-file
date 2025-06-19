using UnityEngine;

public class PixelPointCatcher : MonoBehaviour
{
    private SpriteRenderer sr;
    private int hitCount = 0;
    public int maxHits = 3;

    // 标记是否已被销毁，用于协程判断
    [HideInInspector]
    public bool isDestroyed = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (isDestroyed) return;

        hitCount++;
        Debug.Log("?? Particle hit pixel point at: " + transform.position);

        // 渐变色反馈
        if (sr != null)
        {
            sr.color = Color.Lerp(Color.white, Color.red, hitCount / (float)maxHits);
        }

        if (hitCount >= maxHits)
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
}
