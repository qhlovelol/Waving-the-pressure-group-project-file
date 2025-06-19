using System.Collections.Generic;
using UnityEngine;

public class DualNeedleController : MonoBehaviour
{
    [System.Serializable]
    public class Needle
    {
        public Transform needleTransform;
        public TrailRenderer trailRenderer;
        public bool isCircleDrawer; // ������ǻ�Բ����
        public Color trailColor;

        [HideInInspector] public Vector2 currentInput;
        [HideInInspector] public List<Vector2> positionHistory = new List<Vector2>();
    }

    public Needle needle1; // WASD���ƣ���Բ�룩
    public Needle needle2; // ��������ƣ���Ȧ�룩

    public float moveSpeed = 5f;
    public float circleDetectionInterval = 0.1f;
    public int maxHistoryPoints = 50;
    public float minCircleRadius = 1.0f;
    public float circleVarianceThreshold = 0.3f; // Բ�ε���������Χ

    public GameObject circlePrefab; // ���ɵ�ԲȦԤ����
    public GameObject knotPrefab;   // �����ЧԤ����

    private GameObject activeCircle;
    private float timeSinceLastRecord;
    private bool isDrawingCircle;

    void Start()
    {
        // ��ʼ����βЧ��
        needle1.trailRenderer.startColor = needle1.trailColor;
        needle1.trailRenderer.endColor = new Color(needle1.trailColor.r, needle1.trailColor.g, needle1.trailColor.b, 0);

        needle2.trailRenderer.startColor = needle2.trailColor;
        needle2.trailRenderer.endColor = new Color(needle2.trailColor.r, needle2.trailColor.g, needle2.trailColor.b, 0);
    }

    void Update()
    {
        // ��ȡ�������
        GetPlayerInput();

        // �ƶ���
        MoveNeedle(needle1);
        MoveNeedle(needle2);

        // ��Բ���
        HandleCircleDetection();

        // ��ڼ��
        HandleKnotDetection();
    }

    void GetPlayerInput()
    {
        // WASD���� - �޸���Vector2��������
        float x1 = (Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0));
        float y1 = (Input.GetKey(KeyCode.S) ? -1 : (Input.GetKey(KeyCode.W) ? 1 : 0));
        needle1.currentInput = new Vector2(x1, y1);

        // ��������� - �޸���Vector2��������
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
        // ֻ�л�Բ��Ŵ���Բ���
        if (!needle1.isCircleDrawer) return;

        // ����Ƿ����ڻ���
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

                // ʵʱ����Ƿ�Բ
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
        Debug.Log("��ʼ����");
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
        Debug.Log("��������");
    }

    bool CheckForCircle(List<Vector2> positions)
    {
        if (positions.Count < 10) return false;

        // 1. ����ƽ�����ĵ�
        Vector2 center = Vector2.zero;
        foreach (Vector2 pos in positions)
        {
            center += pos;
        }
        center /= positions.Count;

        // 2. ����ƽ���뾶
        float avgRadius = 0;
        foreach (Vector2 pos in positions)
        {
            avgRadius += Vector2.Distance(pos, center);
        }
        avgRadius /= positions.Count;

        // �뾶̫С����Բ
        if (avgRadius < minCircleRadius) return false;

        // 3. �����Ƿ������Բ��
        float variance = 0;
        foreach (Vector2 pos in positions)
        {
            float distance = Vector2.Distance(pos, center);
            variance += Mathf.Abs(distance - avgRadius);
        }
        variance /= positions.Count;

        // ����İ뾶������Χ
        float maxVariance = avgRadius * circleVarianceThreshold;

        // 4. ��ⷽ��仯��ȷ����Բ���˶���
        Vector2 prevDirection = (positions[1] - positions[0]).normalized;
        int directionChanges = 0;

        for (int i = 2; i < positions.Count; i++)
        {
            Vector2 currentDirection = (positions[i] - positions[i - 1]).normalized;
            float angle = Vector2.SignedAngle(prevDirection, currentDirection);

            // �Ƕȱ仯������ֵʱ����
            if (Mathf.Abs(angle) > 15f)
            {
                directionChanges++;
            }

            prevDirection = currentDirection;
        }

        // ����仯̫�ٿ��ܲ���Բ��
        if (directionChanges < positions.Count * 0.5f)
        {
            return false;
        }

        return variance < maxVariance;
    }

    void GenerateCircle()
    {
        // ����Բ�ĺͰ뾶
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

        // ���ԲȦ�Ѵ���������
        if (activeCircle != null)
        {
            Destroy(activeCircle);
        }

        // ������ԲȦ
        activeCircle = Instantiate(circlePrefab, center, Quaternion.identity);
        activeCircle.transform.localScale = Vector3.one * radius * 2;

        // �����ײ��
        CircleCollider2D collider = activeCircle.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f; // Ԥ�����ǵ�λԲ���뾶Ϊ0.5
        collider.isTrigger = true;

        // ��ӱ�ǩ�Ա���
        activeCircle.tag = "Circle";

        Debug.Log($"����ԲȦ: ����={center}, �뾶={radius}");

        // 5����Զ���ʧ
        Destroy(activeCircle, 5f);
    }

    void HandleKnotDetection()
    {
        if (activeCircle == null) return;

        // ���㴩Ȧ�뵽Բ�ĵľ���
        float distance = Vector2.Distance(
            needle2.needleTransform.position,
            activeCircle.transform.position
        );

        // ��ȡԲȦ��ʵ�ʰ뾶���������ţ�
        float circleRadius = activeCircle.transform.localScale.x / 2; // ��Ϊ������ֱ�������԰뾶����2

        // ����Ȧ�����ԲȦʱ���ɽ��
        if (distance < circleRadius)
        {
            // ���ɽ����Ч
            Vector3 knotPosition = needle2.needleTransform.position;
            Instantiate(knotPrefab, knotPosition, Quaternion.identity);

            Debug.Log("���ɽ��!");

            // ����ԲȦ
            Destroy(activeCircle);
            activeCircle = null;
        }
    }
}