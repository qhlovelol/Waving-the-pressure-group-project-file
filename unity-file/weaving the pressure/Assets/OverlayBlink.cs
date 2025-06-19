using UnityEngine;
using UnityEngine.UI;

public class OverlayBlink : MonoBehaviour
{
    public Image overlayImage;
    public float blinkSpeed = 2f; // ��˸Ƶ��

    private float alphaDirection = 1f;
    private float alpha = 0.3f;

    void Update()
    {
        if (overlayImage == null) return;

        alpha += alphaDirection * blinkSpeed * Time.deltaTime;

        // ����͸������ 0.2 ~ 0.6 ֮�����ر仯
        if (alpha > 0.6f) { alpha = 0.6f; alphaDirection = -1f; }
        if (alpha < 0.2f) { alpha = 0.2f; alphaDirection = 1f; }

        Color c = overlayImage.color;
        c.a = alpha;
        overlayImage.color = c;
    }
}
