using UnityEngine;

public class NeedleController : MonoBehaviour
{
    public float speed = 5f;
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";

    void Update()
    {
        float h = Input.GetAxisRaw(horizontalAxis);
        float v = Input.GetAxisRaw(verticalAxis);
        Vector3 dir = new Vector3(h, v, 0).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }
}

