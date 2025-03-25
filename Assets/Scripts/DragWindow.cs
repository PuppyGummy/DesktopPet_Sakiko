using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class DragWindow : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT { public int X; public int Y; }

    private IntPtr hWnd;
    private bool dragging = false;
    private Vector2 offset;

    void Start()
    {
        hWnd = GetActiveWindow(); // 直接获取窗口句柄
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 右键按下开始拖拽
        {
            POINT cursorPos;
            GetCursorPos(out cursorPos);
            offset = new Vector2(cursorPos.X, cursorPos.Y);
            dragging = true;
        }

        if (Input.GetMouseButtonUp(1)) // 右键松开停止拖拽
        {
            dragging = false;
        }

        if (dragging)
        {
            POINT cursorPos;
            GetCursorPos(out cursorPos);
            int newX = cursorPos.X - (int)offset.x;
            int newY = cursorPos.Y - (int)offset.y;

            MoveWindow(hWnd, newX, newY, Screen.width, Screen.height, true);
        }
    }
}
