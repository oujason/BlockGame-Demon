using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

public class PackWindw : EditorWindow
{
    private string adbPath;
    private string apkPath;

    void OnEnable()
    {
        adbPath = EditorPrefs.GetString("AndroidSdkRoot") + "/platform-tools/adb.exe";
        apkPath = EditorUserSettings.GetConfigValue("apkPath");
    }

    void OnGUI ()
    {
        apkPath = EditorGUILayout.TextField("apk路径:", apkPath);

        if (GUILayout.Button("浏览", GUILayout.Width(80)))
        {
            string path = EditorUtility.OpenFilePanel("选择apk", "", "apk");
            if (!string.IsNullOrEmpty(path))
            {
                apkPath = path;
                this.Repaint();
            }
        }

        EditorUserSettings.SetConfigValue("apkPath", apkPath);

        if (GUILayout.Button("开始安装", GUILayout.Width(280)))
        {
            if (string.IsNullOrEmpty(adbPath))
            {
				//Debug.LogError("请先设置sdk路径");
            }
            else
            {
                Process process = new Process();
                process.StartInfo.FileName = adbPath;
				//process.StartInfo.Arguments = "uninstall " + PlayerSettings.applicationIdentifier;
                process.Start();
				process.StartInfo.Arguments = "install -r " + apkPath;//覆盖安装，而不是卸载再安装，这样就不会删除原有解压出来的资源 20171123
				process.Start();

                this.Close();
            }
        }
    }
}


public class AndroidConnectWindow : EditorWindow
{

    private string adbPath;
    private string mobileIp;
    SerializedProperty pos;

    void OnEnable()
    {
        adbPath = EditorPrefs.GetString("AndroidSdkRoot") + "/platform-tools/adb.exe";
        mobileIp = EditorUserSettings.GetConfigValue("mobileIp");
    }

    void OnGUI ()
    {
        mobileIp = EditorGUILayout.TextField("IP地址：", mobileIp);

        EditorUserSettings.SetConfigValue("mobileIp", mobileIp);

        if (GUILayout.Button("断开连接", GUILayout.Width(280)))
        {
            if (string.IsNullOrEmpty(adbPath))
            {
                //Debug.LogError("请先设置sdk路径");
            }
            else
            {
                Process process = new Process();
                process.StartInfo.FileName = adbPath;
                process.StartInfo.Arguments = "disconnect";
                process.Start();
                this.Close();
            }
        }

        if (GUILayout.Button("连接", GUILayout.Width(280)))
        {
            if (string.IsNullOrEmpty(adbPath))
            {
                //Debug.LogError("请先设置sdk路径");
            }
            else
            {
                Process process = new Process();
                process.StartInfo.FileName = adbPath;
                process.StartInfo.Arguments = "disconnect";
                process.Start();
                process.StartInfo.Arguments = "connect " + mobileIp;
                process.Start();

                this.Close();
            }
        }
    }
}