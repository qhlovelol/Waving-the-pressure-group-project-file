using UnityEngine;

public class CircleNode : MonoBehaviour
{
    public float lifetime = 3f;
    private bool linked = false;
    public GameObject circlePrefab;
    public bool hasSpawnedKnot = false;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void LinkAndConvert(GameObject knotPrefab)
    {
        if (linked) return;
        linked = true;

        Instantiate(knotPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

