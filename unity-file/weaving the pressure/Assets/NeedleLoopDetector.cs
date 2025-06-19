using System.Collections.Generic;
using UnityEngine;

public class NeedleLoopDetectorImproved : MonoBehaviour
{
    public GameObject circlePrefab;
    public float pointSpacing = 0.05f;        // 每隔多少距离记录一点
    public float loopTriggerRadius = 0.4f;    // 距起点多近算闭合 ? 放大一点
    public int minPointsToDetect = 8;         // 防止误触
    public int minClosePasses = 3;            // 至少靠近起点几次才判闭合 ? 新加的逻辑

    private List<Vector3> trailPoints = new List<Vector3>();
    private Vector3 lastPoint;
    private int closePasses = 0;
    private bool loopCreated = false;

    void Update()
    {
        Vector3 current = transform.position;

        if (trailPoints.Count == 0 || Vector3.Distance(current, lastPoint) >= pointSpacing)
        {
            trailPoints.Add(current);
            lastPoint = current;

            CheckLoopClosure();
        }
    }

    void CheckLoopClosure()
    {
        if (loopCreated || trailPoints.Count < minPointsToDetect)
            return;

        float distToStart = Vector3.Distance(trailPoints[0], lastPoint);

        if (distToStart < loopTriggerRadius)
        {
            closePasses++;

            if (closePasses >= minClosePasses)
            {
                // 真正触发闭合
                Debug.Log("?? Loop Detected! Creating Circle.");
                Instantiate(circlePrefab, GetCenter(), Quaternion.identity);
                trailPoints.Clear();
                closePasses = 0;
                loopCreated = true;

                // 延迟一段时间后允许再次触发
                Invoke(nameof(ResetLoopFlag), 1.5f);
            }
        }
    }

    void ResetLoopFlag()
    {
        loopCreated = false;
    }

    Vector3 GetCenter()
    {
        Vector3 sum = Vector3.zero;
        foreach (var p in trailPoints)
            sum += p;
        return sum / trailPoints.Count;
    }
}
