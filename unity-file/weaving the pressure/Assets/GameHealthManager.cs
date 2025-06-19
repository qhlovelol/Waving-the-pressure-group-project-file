using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class GameHealthManager : MonoBehaviour
{
    public int maxHealth = 1000;
    public int currentHealth;

    [Header("血量贴图")]
    public Image overlay90;
    public Image overlay60;
    public Image overlay30;

    [Header("死亡 UI")]
    public GameObject deathPanel;
    public TextMeshProUGUI survivalTimeText;

    [Header("TouchDesigner UDP 设置")]
    public string tdIP = "127.0.0.1";
    public int tdPort = 5055;
    private UdpClient udpSender;

    private float survivalTime = 0f;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        survivalTime = 0f;
        isDead = false;

        if (deathPanel != null)
            deathPanel.SetActive(false);

        UpdateOverlay();

        // 初始化 UDP
        udpSender = new UdpClient();
    }

    void Update()
    {
        if (!isDead)
        {
            survivalTime += Time.unscaledDeltaTime;
        }

        // 按 H 键测试扣血
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(100);
        }

        // ✅ 死后按空格键重启
        if (isDead && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"❤️ Health updated: {currentHealth}/{maxHealth}");

        UpdateOverlay();
        SendHealthToTD(); // ✅ 每次伤害后发送数据

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    void UpdateOverlay()
    {
        float percent = currentHealth / (float)maxHealth;

        if (overlay90 != null) overlay90.gameObject.SetActive(false);
        if (overlay60 != null) overlay60.gameObject.SetActive(false);
        if (overlay30 != null) overlay30.gameObject.SetActive(false);

        if (percent <= 0.3f && overlay30 != null)
            overlay30.gameObject.SetActive(true);
        else if (percent <= 0.6f && overlay60 != null)
            overlay60.gameObject.SetActive(true);
        else if (percent <= 0.9f && overlay90 != null)
            overlay90.gameObject.SetActive(true);
    }

    void HandleDeath()
    {
        Debug.Log("💀 GAME OVER: Health reached 0.");
        isDead = true;

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        if (survivalTimeText != null)
        {
            survivalTimeText.text = $"You survived: {survivalTime:F1} seconds";
        }

        Time.timeScale = 0f;

        // 最后一次发送
        SendHealthToTD();
    }

    public void RestartGame()
    {
        Debug.Log("🔄 Restarting game...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SendHealthToTD()
    {
        if (udpSender == null) return;

        float percent = currentHealth / (float)maxHealth;
        string message = $"health:{percent:F2}";
        byte[] data = Encoding.UTF8.GetBytes(message);

        try
        {
            udpSender.Send(data, data.Length, tdIP, tdPort);
            Debug.Log($"📤 Sent to TD: {message}");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("⚠️ Failed to send UDP: " + ex.Message);
        }
    }

    void OnApplicationQuit()
    {
        udpSender?.Close();
    }
}
