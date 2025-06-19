using UnityEngine;
using System;


public class GreenPoint : MonoBehaviour
{
    public bool isActivated = false;
    private SpriteRenderer sr;

    public Action OnActivated;  // ✅ 回调事件

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Activate()
    {
        if (isActivated) return;

        isActivated = true;
        sr.color = Color.green * 2f;
        gameObject.layer = LayerMask.NameToLayer("ActivatedPoint");

        Debug.Log("✅ 绿点已激活：" + transform.position);

        OnActivated?.Invoke();
    }
}
