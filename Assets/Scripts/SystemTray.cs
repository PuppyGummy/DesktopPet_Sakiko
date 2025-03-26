using UnityEngine;
using System;
using System.Runtime.InteropServices;
#if ENABLE_WINDOWS_FORMS
using System.Windows.Forms;
using ContextMenu = System.Windows.Forms.ContextMenu;
#endif
using System.Drawing;
using Application = UnityEngine.Application;
using SatorImaging.AppWindowUtility;
using System.IO;

public class SystemTray : MonoBehaviour
{
    private NotifyIcon trayIcon;
    private IntPtr hWnd;

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    private const int HWND_TOPMOST = -1;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;

    void Start()
    {
        Windows.HideFromTaskbar();

        // 初始化托盘图标
        trayIcon = new NotifyIcon();

        string iconPath = Path.Combine(Application.streamingAssetsPath, "Sakiko.ico");
        Debug.Log($"Loading icon from: {iconPath}");

        if (!File.Exists(iconPath))
        {
            Debug.LogError("Icon file missing at: " + iconPath);
            trayIcon.Icon = SystemIcons.Application;
        }
        else
        {
            try
            {
                // 使用文件流安全加载
                using (var stream = new FileStream(iconPath, FileMode.Open))
                {
                    Debug.Log("Icon loaded successfully");
                    trayIcon.Icon = new Icon(stream);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load icon: {e.Message}");
                // trayIcon.Icon = SystemIcons.Application;
            }
        }
        // 其他设置
        trayIcon.Text = "Sakiko Desktop Pet";
        trayIcon.Visible = true;

        ContextMenu contextMenu = new ContextMenu();
        contextMenu.MenuItems.Add("退出", ExitApp);
        trayIcon.ContextMenu = contextMenu;

        trayIcon.DoubleClick += (sender, e) =>
        {
            SetWindowPos(hWnd, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE);
        };
    }

    void OnApplicationQuit()
    {
        if (trayIcon != null)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
        }
    }

    private void ExitApp(object sender, EventArgs e)
    {
        Application.Quit();
    }
}