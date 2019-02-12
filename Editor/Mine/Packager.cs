using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Packager
{
    /// Resources目录，打包时将Resouces改名为Resource
    static string resourcePath = "Assets/Resource/";

    /// 要打包的资源文件列表
    static List<string> resourceFilePaths = new List<string>();

    /// 打包后的所有资源文件列表
    static List<string> files = new List<string>();
    /// 打包列表
    static List<AssetBundleBuild> maps = new List<AssetBundleBuild>();

    /// <summary>
    /// 开始打包
    /// </summary>
    [MenuItem("打包/打包AssetBundle资源 %j")]
    public static void BuildAssetResource()
    {
        #if UNITY_STANDALONE_WIN
        BuildTarget target = BuildTarget.StandaloneWindows;
        #elif UNITY_ANDROID
        BuildTarget target = BuildTarget.Android;
        #elif UNITY_IOS
        BuildTarget target = BuildTarget.iOS;
        #endif
        if (Directory.Exists(GameEnvironment.DataPath))     //删除Patch目录
        {
            Directory.Delete(GameEnvironment.DataPath, true);
        }
        string streamPath = Application.streamingAssetsPath;
        if (Directory.Exists(streamPath))                   //删除StreamingAssets目录
        {
            Directory.Delete(streamPath, true);
        }

        Directory.CreateDirectory(streamPath);          //创建StreamingAssets目录
        AssetDatabase.Refresh();                        //刷新Priject视图

        maps.Clear();                                   //清空打包列表
        resourceFilePaths.Clear();

        string resPath = Application.dataPath.ToLower() + "/StreamingAssets/";      //打包输出目录
        if (!Directory.Exists(resPath))
            Directory.CreateDirectory(resPath);

        AddResourceFile(resourcePath);                  //添加需要打包的资源

        BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
        BuildPipeline.BuildAssetBundles(resPath, maps.ToArray(), options, target);  //开始打包

        AssetDatabase.Refresh();    //刷新Priject视图
    }

    /// <summary>
    /// 添加打包后的资源列表
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta"))
                continue;
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            Recursive(dir);
        }
    }

    /// <summary>
    /// 添加需要打包的资源
    /// </summary>
    static void AddResourceFile(string path)
    {
        string[] files = Directory.GetFiles(path);
        if (files.Length > 0 && path != resourcePath)  //存在文件，为该文件目录创建一个ab，Reource跟目录中不能放文件
        {
            AssetBundleBuild build = new AssetBundleBuild();
            string rectPath = path.Replace('\\', '/');
            build.assetBundleName = rectPath.Replace(resourcePath, "") + ".unity3d";
            List<string> assetNames = new List<string>();
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Replace('\\', '/');
                string ext = Path.GetExtension(files[i]);
                if (!ext.Equals(".meta"))
                {
                    assetNames.Add(files[i]);
                    resourceFilePaths.Add(files[i]);

                    string[] dependencies = AssetDatabase.GetDependencies(new string[]{ files[i] });    //查询要打包的资源所有依赖的资源
                    foreach (string dependence in dependencies)
                    {
                        if (dependence == files[i] || Path.GetExtension(dependence) == ".cs" || resourceFilePaths.Contains(dependence) || dependence.Contains(resourcePath))    //依赖的资源为自己，为cs脚本，已打包，在Resource中都不再打包
                            continue;
                        AssetBundleBuild build1 = new AssetBundleBuild();
						build1.assetBundleName = Path.GetDirectoryName(dependence) + ".unity3d";
                        build1.assetNames = new string[]{dependence};
                        maps.Add(build1);
                        resourceFilePaths.Add(dependence);      //非Resource中的依赖资源独立打包
                    }
                }
            }

            build.assetNames = assetNames.ToArray();
            maps.Add(build);
        }

        string[] dirs = Directory.GetDirectories(path);
        foreach (string dir in dirs)
        {
            AddResourceFile(dir);
        }
    }
}