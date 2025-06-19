using UnityEngine;

public class PixelPointCatcher : MonoBehaviour
{
    private SpriteRenderer sr;
    private int hitCount = 0;
    public int maxHits = 3;

    // ����Ƿ��ѱ����٣�����Э���ж�
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

        // ����ɫ����
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
