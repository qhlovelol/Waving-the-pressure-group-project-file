using System.Collections.Generic;
using UnityEngine;

public class DrawingDetector : MonoBehaviour
{
    public float recordInterval = 0.1f; // 记录点的时间间隔
    public int maxPoints = 50; // 最大记录点数
    public float minCircleRadius = 1.0f; // 最小圆半径

    private List<Vector2> recordedPositions = new List<Vector2>();
    private float timeSinceLastRecord;
    private bool isDrawing = false;

    void Update()
    {
        // 获取玩家输入（示例，需根据实际输入修改）
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

            // 实时检测是否画圆
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
        // 最终检测
        if (CheckForCircle())
        {
            Debug.Log("Circle drawn!");
            // 在这里生成可见的圆
        }
    }

    bool CheckForCircle()
    {
        if (recordedPositions.Count < 10) return false;

        // 1. 计算平均中心点
        Vector2 center = Vector2.zero;
        foreach (Vector2 pos in recordedPositions)
        {
            center += pos;
        }
        center /= recordedPositions.Count;

        // 2. 计算平均半径
        float avgRadius = 0;
        foreach (Vector2 pos in recordedPositions)
        {
            avgRadius += Vector2.Distance(pos, center);
        }
        avgRadius /= recordedPositions.Count;

        if (avgRadius < minCircleRadius) return false;

        // 3. 检查点是否大致在圆上
        float variance = 0;
        foreach (Vector2 pos in recordedPositions)
        {
            float distance = Vector2.Distance(pos, center);
            variance += Mathf.Abs(distance - avgRadius);
        }
        variance /= recordedPositions.Count;

        // 允许的半径波动范围
        float maxVariance = avgRadius * 0.3f;

        return variance < maxVariance;
    }
}