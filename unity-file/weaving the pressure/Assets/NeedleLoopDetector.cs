using System.Collections.Generic;
using UnityEngine;

public class NeedleLoopDetectorImproved : MonoBehaviour
{
    public GameObject circlePrefab;
    public float pointSpacing = 0.05f;        // ÿ�����پ����¼һ��
    public float loopTriggerRadius = 0.4f;    // ���������պ� ? �Ŵ�һ��
    public int minPointsToDetect = 8;         // ��ֹ��
    public int minClosePasses = 3;            // ���ٿ�����㼸�β��бպ� ? �¼ӵ��߼�

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
                // ���������պ�
                Debug.Log("?? Loop Detected! Creating Circle.");
                Instantiate(circlePrefab, GetCenter(), Quaternion.identity);
                trailPoints.Clear();
                closePasses = 0;
                loopCreated = true;

                // �ӳ�һ��ʱ��������ٴδ���
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
