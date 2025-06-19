using UnityEngine;

public class WaterfallSpawner : MonoBehaviour
{
    public GameObject dropletPrefab;

    [Header("生成控制")]
    public float initialRate = 10f;           // 初始生成速率（每秒）
    public float maxRate = 200f;              // 最大生成速率
    public float densityIncreaseSpeed = 10f;  // 每秒粒子数增加

    [Header("宽度控制")]
    public float initialWidth = 1f;
    public float maxWidth = 10f;
    public float widthExpandSpeed = 1f;       // 每秒宽度增加

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
        // 更新生成频率和宽度
        currentRate = Mathf.Min(currentRate + densityIncreaseSpeed * Time.deltaTime, maxRate);
        currentWidth = Mathf.Min(currentWidth + widthExpandSpeed * Time.deltaTime, maxWidth);

        // 按当前速率生成 droplet
        spawnTimer += Time.deltaTime;
        float interval = 1f / currentRate;

        while (spawnTimer >= interval)
        {
            spawnTimer -= interval;

            // 在 currentWidth 范围内生成 droplet
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-currentWidth / 2f, currentWidth / 2f), 0f, 0f);
            Instantiate(dropletPrefab, spawnPos, Quaternion.identity);
        }
    }
}
