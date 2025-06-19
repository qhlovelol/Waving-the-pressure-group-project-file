using UnityEngine;

public class GreenPointSpawner : MonoBehaviour
{
    public GameObject greenPointPrefab;     // 拖入绿色像素点预制体
    public float spawnInterval = 2f;        // 每隔几秒生成一个
    public int maxPoints = 20;              // 最多存在几个点
    public Vector2 spawnAreaMin = new Vector2(-4f, 0f);  // 左下角
    public Vector2 spawnAreaMax = new Vector2(4f, 4f);   // 右上角

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

        // 自动减少计数：当它被激活或销毁
        GreenPoint gp = go.GetComponent<GreenPoint>();
        if (gp != null)
        {
            gp.OnActivated += () => currentPoints--;
        }
    }
}
