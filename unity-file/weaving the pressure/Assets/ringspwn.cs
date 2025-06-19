using UnityEngine;

public class NeedleACollider : MonoBehaviour
{
    public GameObject knotPrefab; // 要生成的结节 Prefab

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("circle"))
        {
            CircleNode node = other.GetComponent<CircleNode>();

            if (node != null && !node.hasSpawnedKnot)
            {
                Vector3 spawnPos = transform.position + new Vector3(0.5f, 0, 0); // 可调整偏移
                Instantiate(knotPrefab, spawnPos, Quaternion.identity);

                node.hasSpawnedKnot = true;  // ✅ 标记为已生成
                Debug.Log("🧶 Knot created on ring (once only).");
            }
        }
    }
}
