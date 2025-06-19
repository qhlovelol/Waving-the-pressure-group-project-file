using UnityEngine;

public class NeedleBDetect : MonoBehaviour
{
    public string targetTag = "circle";
    public GameObject knotPrefab;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            CircleNode node = other.GetComponent<CircleNode>();
            if (node != null)
            {
                node.LinkAndConvert(knotPrefab);
            }
        }
    }

}
