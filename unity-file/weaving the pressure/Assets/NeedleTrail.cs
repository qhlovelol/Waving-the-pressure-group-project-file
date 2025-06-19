using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleTrail : MonoBehaviour
{
    [Header("��β����")]
    public GameObject linePrefab;             // ��β���Ԥ���壨���ص㣩
    public Transform anchorPoint;             // ��ͷλ�ã�ͨ���� A ���ǰ�ˣ�
    public float pointSpacing = 0.1f;         // ��β�����ɵ���С������

    private Vector3 lastPoint;                // ��һ����¼��

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

            // ��β��������
            point.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        if (point != null)
            Destroy(point);
    }
}
