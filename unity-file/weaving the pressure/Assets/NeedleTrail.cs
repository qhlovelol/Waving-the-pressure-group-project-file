using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleTrail : MonoBehaviour
{
    [Header("拖尾设置")]
    public GameObject linePrefab;             // 拖尾点的预制体（像素点）
    public Transform anchorPoint;             // 针头位置（通常是 A 针的前端）
    public float pointSpacing = 0.1f;         // 拖尾点生成的最小距离间隔

    private Vector3 lastPoint;                // 上一个记录点

    void Start()
    {
        lastPoint = anchorPoint.position;
        StartCoroutine(CreateTrail());
    }

    IEnumerator CreateTrail()
    {
        while (true)
        {
            float dist = Vector3.Distance(anchorPoint.position, lastPoint);

            if (dist >= pointSpacing)
            {
                GameObject newPoint = Instantiate(linePrefab, anchorPoint.position, Quaternion.identity);
                StartCoroutine(MoveAndDestroy(newPoint));
                lastPoint = anchorPoint.position;
            }

            yield return null;
        }
    }

    IEnumerator MoveAndDestroy(GameObject point)
    {
        float lifetime = 3f;
        float timer = 0f;
        float fallSpeed = 0.2f;

        while (timer < lifetime)
        {
            if (point == null) yield break;

            // 拖尾点逐渐下落
            point.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        if (point != null)
            Destroy(point);
    }
}
