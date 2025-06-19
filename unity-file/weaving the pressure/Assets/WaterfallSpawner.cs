using UnityEngine;

public class WaterfallSpawner : MonoBehaviour
{
    public GameObject dropletPrefab;

    [Header("���ɿ���")]
    public float initialRate = 10f;           // ��ʼ�������ʣ�ÿ�룩
    public float maxRate = 200f;              // �����������
    public float densityIncreaseSpeed = 10f;  // ÿ������������

    [Header("��ȿ���")]
    public float initialWidth = 1f;
    public float maxWidth = 10f;
    public float widthExpandSpeed = 1f;       // ÿ��������

    private float spawnTimer = 0f;
    private float currentRate;
    private float currentWidth;

    void Start()
    {
        currentRate = initialRate;
        currentWidth = initialWidth;
    }

    void Update()
    {
        // ��������Ƶ�ʺͿ��
        currentRate = Mathf.Min(currentRate + densityIncreaseSpeed * Time.deltaTime, maxRate);
        currentWidth = Mathf.Min(currentWidth + widthExpandSpeed * Time.deltaTime, maxWidth);

        // ����ǰ�������� droplet
        spawnTimer += Time.deltaTime;
        float interval = 1f / currentRate;

        while (spawnTimer >= interval)
        {
            spawnTimer -= interval;

            // �� currentWidth ��Χ������ droplet
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-currentWidth / 2f, currentWidth / 2f), 0f, 0f);
            Instantiate(dropletPrefab, spawnPos, Quaternion.identity);
        }
    }
}
