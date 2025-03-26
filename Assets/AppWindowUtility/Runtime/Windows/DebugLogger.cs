using UnityEngine;
using UnityEngine.UI;

public class DebugLogger : MonoBehaviour
{
    public Text logText;
    public static DebugLogger Instance { get; private set; }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Log(string message)
    {
        logText.text = message;
    }
}