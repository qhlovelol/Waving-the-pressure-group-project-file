using UnityEngine;

public class NeedleBController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        GreenPoint green = other.GetComponent<GreenPoint>();
        if (green != null && !green.isActivated)
        {
            Debug.Log("🧷 B针触碰到了一个绿点：" + green.name);
            green.Activate(); // 👈 会打印“绿点已激活”
            GreenPointConnector.Instance.RegisterPoint(green.transform.position);
        }
    }
}
