using System.Collections.Generic;
using UnityEngine;

public class DrawingDetector : MonoBehaviour
{
    public float recordInterval = 0.1f; // ��¼���ʱ����
    public int maxPoints = 50; // ����¼����
    public float minCircleRadius = 1.0f; // ��СԲ�뾶

    private List<Vector2> recordedPositions = new List<Vector2>();
    private float timeSinceLastRecord;
    private bool isDrawing = false;

    void Update()
    {
        // ��ȡ������루ʾ���������ʵ�������޸ģ�
        Vector2 input = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        if (input.magnitude > 0.1f)
        {
            if (!isDrawing)
            {
                StartDrawing();
            }
            RecordPosition(transform.position);
        }
        else
        {
            if (isDrawing)
            {
                StopDrawing();
            }
        }

        if (isDrawing)
        {
            timeSinceLastRecord += Time.deltaTime;
            if (timeSinceLastRecord >= recordInterval)
            {
                RecordPosition(transform.position);
                timeSinceLastRecord = 0;
            }

            // ʵʱ����Ƿ�Բ
            CheckForCircle();
        }
    }

    void StartDrawing()
    {
        recordedPositions.Clear();
        isDrawing = true;
    }

    void RecordPosition(Vector3 position)
    {
        recordedPositions.Add(position);
        if (recordedPositions.Count > maxPoints)
        {
            recordedPositions.RemoveAt(0);
        }
    }

    void StopDrawing()
    {
        isDrawing = false;
        // ���ռ��
        if (CheckForCircle())
        {
            Debug.Log("Circle drawn!");
            // ���������ɿɼ���Բ
        }
    }

    bool CheckForCircle()
    {
        if (recordedPositions.Count < 10) return false;

        // 1. ����ƽ�����ĵ�
        Vector2 center = Vector2.zero;
        foreach (Vector2 pos in recordedPositions)
        {
            center += pos;
        }
        center /= recordedPositions.Count;

        // 2. ����ƽ���뾶
        float avgRadius = 0;
        foreach (Vector2 pos in recordedPositions)
        {
            avgRadius += Vector2.Distance(pos, center);
        }
        avgRadius /= recordedPositions.Count;

        if (avgRadius < minCircleRadius) return false;

        // 3. �����Ƿ������Բ��
        float variance = 0;
        foreach (Vector2 pos in recordedPositions)
        {
            float distance = Vector2.Distance(pos, center);
            variance += Mathf.Abs(distance - avgRadius);
        }
        variance /= recordedPositions.Count;

        // ����İ뾶������Χ
        float maxVariance = avgRadius * 0.3f;

        return variance < maxVariance;
    }
}