using UnityEngine;
using UnityEngine.UI;

public class OverlayBlink : MonoBehaviour
{
    public Image overlayImage;
    public float blinkSpeed = 2f; // 闪烁频率

    private float alphaDirection = 1f;
    private float alpha = 0.3f;

    void Update()
    {
        if (overlayImage == null) return;

        alpha += alphaDirection * blinkSpeed * Time.deltaTime;

        // 控制透明度在 0.2 ~ 0.6 之间来回变化
        if (alpha > 0.6f) { alpha = 0.6f; alphaDirection = -1f; }
        if (alpha < 0.2f) { alpha = 0.2f; alphaDirection = 1f; }

        Color c = overlayImage.color;
        c.a = alpha;
        overlayImage.color = c;
    }
}
