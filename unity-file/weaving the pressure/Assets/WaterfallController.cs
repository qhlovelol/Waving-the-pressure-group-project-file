using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class WaterfallController : MonoBehaviour
{
    public float widthExpandSpeed = 1f;      // 每秒宽度增加
    public float densityIncreaseSpeed = 10f; // 每秒粒子数增加
    public float maxWidth = 10f;
    public float maxRate = 200f;

    private ParticleSystem ps;
    private ParticleSystem.EmissionModule emission;
    private ParticleSystem.ShapeModule shape;

    private float currentWidth = 1f;
    private float currentRate = 50f;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        emission = ps.emission;
        shape = ps.shape;

        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(currentWidth, 0.1f, 0.1f);
        emission.rateOverTime = currentRate;
    }

    void Update()
    {
        // 宽度递增
        currentWidth = Mathf.Min(currentWidth + widthExpandSpeed * Time.deltaTime, maxWidth);
        shape.scale = new Vector3(currentWidth, 0.1f, 0.1f);

        // 发射速率递增
        currentRate = Mathf.Min(currentRate + densityIncreaseSpeed * Time.deltaTime, maxRate);
        emission.rateOverTime = currentRate;
    }
}
