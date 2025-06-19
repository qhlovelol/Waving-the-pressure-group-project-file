using UnityEngine;

public class GreenPointSpawner : MonoBehaviour
{
    public GameObject greenPointPrefab;     // ������ɫ���ص�Ԥ����
    public float spawnInterval = 2f;        // ÿ����������һ��
    public int maxPoints = 20;              // �����ڼ�����
    public Vector2 spawnAreaMin = new Vector2(-4f, 0f);  // ���½�
    public Vector2 spawnAreaMax = new Vector2(4f, 4f);   // ���Ͻ�

    private int currentPoints = 0;

    void Start()
    {
        InvokeRepeating(nameof(SpawnPoint), 1f, spawnInterval);
    }

    void SpawnPoint()
    {
        if (currentPoints >= maxPoints) return;

        Vector2 pos = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        GameObject go = Instantiate(greenPointPrefab, pos, Quaternion.identity);
        currentPoints++;

        // �Զ����ټ��������������������
        GreenPoint gp = go.GetComponent<GreenPoint>();
        if (gp != null)
        {
            gp.OnActivated += () => currentPoints--;
        }
    }
}
