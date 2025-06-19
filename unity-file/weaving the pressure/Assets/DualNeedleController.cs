using System.Collections.Generic;
using UnityEngine;

public class DualNeedleController : MonoBehaviour
{
    [System.Serializable]
    public class Needle
    {
        public Transform needleTransform;
        public TrailRenderer trailRenderer;
        public bool isCircleDrawer; // 标记这是画圆的针
        public Color trailColor;

        [HideInInspector] public Vector2 currentInput;
        [HideInInspector] public List<Vector2> positionHistory = new List<Vector2>();
    }

    public Needle needle1; // WASD控制（画圆针）
    public Needle needle2; // 方向键控制（穿圈针）

    public float moveSpeed = 5f;
    public float circleDetectionInterval = 0.1f;
    public int maxHistoryPoints = 50;
    public float minCircleRadius = 1.0f;
    public float circleVarianceThreshold = 0.3f; // 圆形的允许波动范围

    public GameObject circlePrefab; // 生成的圆圈预制体
    public GameObject knotPrefab;   // 结节特效预制体

    private GameObject activeCircle;
    private float timeSinceLastRecord;
    private bool isDrawingCircle;

    void Start()
    {
        // 初始化拖尾效果
        needle1.trailRenderer.startColor = needle1.trailColor;
        needle1.trailRenderer.endColor = new Color(needle1.trailColor.r, needle1.trailColor.g, needle1.trailColor.b, 0);

        needle2.trailRenderer.startColor = needle2.trailColor;
        needle2.trailRenderer.endColor = new Color(needle2.trailColor.r, needle2.trailColor.g, needle2.trailColor.b, 0);
    }

    void Update()
    {
        // 获取玩家输入
        GetPlayerInput();

        // 移动针
        MoveNeedle(needle1);
        MoveNeedle(needle2);

        // 画圆检测
        HandleCircleDetection();

        // 结节检测
        HandleKnotDetection();
    }

    void GetPlayerInput()
    {
        // WASD控制 - 修复了Vector2参数错误
        float x1 = (Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0));
        float y1 = (Input.GetKey(KeyCode.S) ? -1 : (Input.GetKey(KeyCode.W) ? 1 : 0));
        needle1.currentInput = new Vector2(x1, y1);

        // 方向键控制 - 修复了Vector2参数错误
        float x2 = (Input.GetKey(KeyCode.LeftArrow) ? -1 : (Input.GetKey(KeyCode.RightArrow) ? 1 : 0));
        float y2 = (Input.GetKey(KeyCode.DownArrow) ? -1 : (Input.GetKey(KeyCode.UpArrow) ? 1 : 0));
        needle2.currentInput = new Vector2(x2, y2);
    }

    void MoveNeedle(Needle needle)
    {
        if (needle.currentInput.magnitude > 0.1f)
        {
            Vector3 movement = new Vector3(needle.currentInput.x, needle.currentInput.y, 0) * moveSpeed * Time.deltaTime;
            needle.needleTransform.position += movement;
        }
    }

    void HandleCircleDetection()
    {
        // 只有画圆针才处理画圆检测
        if (!needle1.isCircleDrawer) return;

        // 检查是否正在绘制
        if (needle1.currentInput.magnitude > 0.1f)
        {
            if (!isDrawingCircle)
            {
                StartDrawingCircle();
            }

            timeSinceLastRecord += Time.deltaTime;
            if (timeSinceLastRecord >= circleDetectionInterval)
            {
                RecordPosition(needle1);
                timeSinceLastRecord = 0;

                // 实时检测是否画圆
                if (CheckForCircle(needle1.positionHistory))
                {
                    GenerateCircle();
                }
            }
        }
        else
        {
            if (isDrawingCircle)
            {
                StopDrawingCircle();
            }
        }
    }

    void StartDrawingCircle()
    {
        needle1.positionHistory.Clear();
        isDrawingCircle = true;
        Debug.Log("开始绘制");
    }

    void RecordPosition(Needle needle)
    {
        needle.positionHistory.Add(needle.needleTransform.position);
        if (needle.positionHistory.Count > maxHistoryPoints)
        {
            needle.positionHistory.RemoveAt(0);
        }
    }

    void StopDrawingCircle()
    {
        isDrawingCircle = false;
        Debug.Log("结束绘制");
    }

    bool CheckForCircle(List<Vector2> positions)
    {
        if (positions.Count < 10) return false;

        // 1. 计算平均中心点
        Vector2 center = Vector2.zero;
        foreach (Vector2 pos in positions)
        {
            center += pos;
        }
        center /= positions.Count;

        // 2. 计算平均半径
        float avgRadius = 0;
        foreach (Vector2 pos in positions)
        {
            avgRadius += Vector2.Distance(pos, center);
        }
        avgRadius /= positions.Count;

        // 半径太小不算圆
        if (avgRadius < minCircleRadius) return false;

        // 3. 检查点是否大致在圆上
        float variance = 0;
        foreach (Vector2 pos in positions)
        {
            float distance = Vector2.Distance(pos, center);
            variance += Mathf.Abs(distance - avgRadius);
        }
        variance /= positions.Count;

        // 允许的半径波动范围
        float maxVariance = avgRadius * circleVarianceThreshold;

        // 4. 检测方向变化（确保是圆形运动）
        Vector2 prevDirection = (positions[1] - positions[0]).normalized;
        int directionChanges = 0;

        for (int i = 2; i < positions.Count; i++)
        {
            Vector2 currentDirection = (positions[i] - positions[i - 1]).normalized;
            float angle = Vector2.SignedAngle(prevDirection, currentDirection);

            // 角度变化大于阈值时计数
            if (Mathf.Abs(angle) > 15f)
            {
                directionChanges++;
            }

            prevDirection = currentDirection;
        }

        // 方向变化太少可能不是圆形
        if (directionChanges < positions.Count * 0.5f)
        {
            return false;
        }

        return variance < maxVariance;
    }

    void GenerateCircle()
    {
        // 计算圆心和半径
        Vector2 center = Vector2.zero;
        foreach (Vector2 pos in needle1.positionHistory)
        {
            center += pos;
        }
        center /= needle1.positionHistory.Count;

        float radius = 0;
        foreach (Vector2 pos in needle1.positionHistory)
        {
            radius += Vector2.Distance(pos, center);
        }
        radius /= needle1.positionHistory.Count;

        // 如果圆圈已存在则销毁
        if (activeCircle != null)
        {
            Destroy(activeCircle);
        }

        // 创建新圆圈
        activeCircle = Instantiate(circlePrefab, center, Quaternion.identity);
        activeCircle.transform.localScale = Vector3.one * radius * 2;

        // 添加碰撞器
        CircleCollider2D collider = activeCircle.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f; // 预制体是单位圆，半径为0.5
        collider.isTrigger = true;

        // 添加标签以便检测
        activeCircle.tag = "Circle";

        Debug.Log($"生成圆圈: 中心={center}, 半径={radius}");

        // 5秒后自动消失
        Destroy(activeCircle, 5f);
    }

    void HandleKnotDetection()
    {
        if (activeCircle == null) return;

        // 计算穿圈针到圆心的距离
        float distance = Vector2.Distance(
            needle2.needleTransform.position,
            activeCircle.transform.position
        );

        // 获取圆圈的实际半径（考虑缩放）
        float circleRadius = activeCircle.transform.localScale.x / 2; // 因为缩放是直径，所以半径除以2

        // 当穿圈针进入圆圈时生成结节
        if (distance < circleRadius)
        {
            // 生成结节特效
            Vector3 knotPosition = needle2.needleTransform.position;
            Instantiate(knotPrefab, knotPosition, Quaternion.identity);

            Debug.Log("生成结节!");

            // 销毁圆圈
            Destroy(activeCircle);
            activeCircle = null;
        }
    }
}