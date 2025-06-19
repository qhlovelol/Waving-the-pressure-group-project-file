using UnityEngine;

public class NeedleArrows : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) h = -1;
        if (Input.GetKey(KeyCode.RightArrow)) h = 1;
        if (Input.GetKey(KeyCode.UpArrow)) v = 1;
        if (Input.GetKey(KeyCode.DownArrow)) v = -1;

        Vector3 dir = new Vector3(h, v, 0).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }
}

