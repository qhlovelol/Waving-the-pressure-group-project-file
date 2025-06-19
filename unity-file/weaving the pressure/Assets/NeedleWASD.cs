using UnityEngine;

public class NeedleWASD : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.A)) h = -1;
        if (Input.GetKey(KeyCode.D)) h = 1;
        if (Input.GetKey(KeyCode.W)) v = 1;
        if (Input.GetKey(KeyCode.S)) v = -1;

        Vector3 dir = new Vector3(h, v, 0).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }
}
