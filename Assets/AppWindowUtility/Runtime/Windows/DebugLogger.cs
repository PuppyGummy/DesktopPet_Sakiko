using UnityEngine;
using UnityEngine.UI;

public class DebugLogger : MonoBehaviour
{
    public static Text logText;
    //singleton

    void Start()
    {
        logText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public static void Log(string message)
    {
        logText.text = message;
    }
}