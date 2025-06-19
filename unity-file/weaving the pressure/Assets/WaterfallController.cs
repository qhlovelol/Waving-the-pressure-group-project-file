using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class WaterfallController : MonoBehaviour
{
    public float widthExpandSpeed = 1f;      // ÿ��������
    public float densityIncreaseSpeed = 10f; // ÿ������������
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
        // ��ȵ���
        currentWidth = Mathf.Min(currentWidth + widthExpandSpeed * Time.deltaTime, maxWidth);
        shape.scale = new Vector3(currentWidth, 0.1f, 0.1f);

        // �������ʵ���
        currentRate = Mathf.Min(currentRate + densityIncreaseSpeed * Time.deltaTime, maxRate);
        emission.rateOverTime = currentRate;
    }
}
