using System.Collections.Generic;
using UnityEngine;

public class GreenPointConnector : MonoBehaviour
{
    public static GreenPointConnector Instance;

    public Material lineMaterial;
    private List<Vector3> activatedPoints = new List<Vector3>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterPoint(Vector3 position)
    {
        activatedPoints.Add(position);
        Debug.Log("🟩 注册激活点：" + position);
        DrawLine();
    }

    void DrawLine()
    {
        if (activatedPoints.Count < 2)
        {
            Debug.Log("❕点数不足，不绘制线（当前：" + activatedPoints.Count + ")");
            return;
        }

        GameObject lineObj = new GameObject("GreenLine");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.positionCount = activatedPoints.Count;
        lr.SetPositions(activatedPoints.ToArray());

        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = lineMaterial;
        lr.startColor = Color.green;
        lr.endColor = Color.green;
        lr.sortingOrder = 10;

        Debug.Log("📏 已绘制绿线，共 " + activatedPoints.Count + " 个点");
    }
}
