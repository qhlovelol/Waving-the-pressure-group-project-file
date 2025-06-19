using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;

public class HandUDPReceiver : MonoBehaviour
{
    UdpClient client;
    Thread receiveThread;
    bool isReceiving = true;

    public Transform rightHandObject;
    public GameObject knotPrefab;
    public Transform leftFingerObject;

    private float knotCooldown = 2f;
    private float lastKnotTime = -999f;

    private Vector2 rightTarget = Vector2.zero;
    private bool hasRight = false;
    private float lostRightTimer = 0f;
    private float rightDataTimeout = 0.5f;

    private Vector2 leftFingerPos = Vector2.zero;
    private bool hasLeftFinger = false;

    private volatile bool shouldCreateKnot = false;

    void Start()
    {
        client = new UdpClient(5053);
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log("✅ UDP Receiver Started on port 5053");
    }

    void Update()
    {
        if (rightHandObject != null)
        {
            if (hasRight)
            {
                rightHandObject.position = Vector2.Lerp(rightHandObject.position, rightTarget, 0.6f);
            }
            else
            {
                lostRightTimer += Time.deltaTime;
                if (lostRightTimer < rightDataTimeout)
                {
                    rightHandObject.position = Vector2.Lerp(rightHandObject.position, rightTarget, 0.1f);
                }
            }
        }

        if (leftFingerObject != null && hasLeftFinger)
        {
            leftFingerObject.position = Vector2.Lerp(leftFingerObject.position, leftFingerPos, 0.5f);
        }

        if (shouldCreateKnot)
        {
            CreateKnot();
            shouldCreateKnot = false;
        }

        // 🔁 支持空格键手动重启
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    void ReceiveData()
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 5053);

        while (isReceiving)
        {
            try
            {
                if (client.Available > 0)
                {
                    byte[] data = client.Receive(ref ep);
                    string rawJson = Encoding.UTF8.GetString(data);
                    Debug.Log("📩 Received JSON: " + rawJson);

                    string wrapped = WrapJson(rawJson);
                    Debug.Log("📦 Wrapped JSON: " + wrapped);

                    var wrapper = JsonUtility.FromJson<Wrapper>(wrapped);

                    if (wrapper.Right != null && wrapper.Right.x > 0 && wrapper.Right.y > 0)
                    {
                        rightTarget = Normalize(wrapper.Right.x, wrapper.Right.y);
                        hasRight = true;
                        lostRightTimer = 0f;
                    }
                    else
                    {
                        hasRight = false;
                    }

                    if (wrapper.LeftFinger != null && wrapper.LeftFinger.x > 0 && wrapper.LeftFinger.y > 0)
                    {
                        leftFingerPos = Normalize(wrapper.LeftFinger.x, wrapper.LeftFinger.y);
                        hasLeftFinger = true;
                    }
                    else
                    {
                        hasLeftFinger = false;
                    }

                    if (wrapper.LeftGesture != null && wrapper.LeftGesture == "fist")
                    {
                        shouldCreateKnot = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("❌ JSON Parse Error: " + ex.Message);
            }
        }
    }

    void CreateKnot()
    {
        if (Time.time - lastKnotTime < knotCooldown) return;

        if (leftFingerObject != null && knotPrefab != null)
        {
            Vector3 spawnPos = leftFingerObject.position;
            Instantiate(knotPrefab, spawnPos, Quaternion.identity);
            lastKnotTime = Time.time;
            Debug.Log("🧶 Knot created at: " + spawnPos);
        }
    }

    void RestartGame()
    {
        Debug.Log("🔁 Restarting...");
        Time.timeScale = 1f;
        StopReceiver();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void StopReceiver()
    {
        isReceiving = false;
        try
        {
            client?.Close();
        }
        catch { }
        Debug.Log("🛑 UDP Receiver Closed.");
    }

    void OnApplicationQuit()
    {
        StopReceiver();
    }

    Vector2 Normalize(float x, float y)
    {
        return new Vector2((x - 0.5f) * 20f, (0.5f - y) * 12f);
    }

    [System.Serializable] class Hand { public float x, y; }

    [System.Serializable]
    class Wrapper
    {
        public Hand Right;
        public string LeftGesture;
        public Hand LeftFinger;
    }

    string WrapJson(string raw)
    {
        string right = ExtractHandSafe(raw, "Right");
        string gesture = ExtractGestureSafe(raw, "LeftGesture");
        string leftFinger = ExtractHandSafe(raw, "LeftFinger");
        return $"{{\"Right\":{right},\"LeftGesture\":{gesture},\"LeftFinger\":{leftFinger}}}";
    }

    string ExtractHandSafe(string json, string key)
    {
        int start = json.IndexOf($"\"{key}\"");
        if (start == -1) return "null";
        int braceStart = json.IndexOf("{", start);
        int braceEnd = json.IndexOf("}", braceStart);
        if (braceStart == -1 || braceEnd == -1) return "null";
        return json.Substring(braceStart, braceEnd - braceStart + 1);
    }

    string ExtractGestureSafe(string json, string key)
    {
        int start = json.IndexOf($"\"{key}\"");
        if (start == -1) return "null";
        int quoteStart = json.IndexOf("\"", start + key.Length + 2);
        int quoteEnd = json.IndexOf("\"", quoteStart + 1);
        return "\"" + json.Substring(quoteStart + 1, quoteEnd - quoteStart - 1) + "\"";
    }
}
