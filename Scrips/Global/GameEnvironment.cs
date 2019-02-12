//#define LOAD_FROM_STREAMINGASSETS
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UObject = UnityEngine.Object;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates; 
using System.Text;
using System.Linq;

public class AssetBundleInfo
{
    public AssetBundle m_AssetBundle;
    public int m_ReferencedCount;

    public AssetBundleInfo(AssetBundle assetBundle)
    {
        m_AssetBundle = assetBundle;
        m_ReferencedCount = 0;
    }
}

public class GameEnvironment : MonoBehaviour
{
    public delegate void ScenceLoadCb(GameObject uiLayer);

    private static GameEnvironment instance = null;

    public static GameEnvironment GetInstance { get { return instance; } }

    private static string dataPath = null;

    public Camera UICam;

    [HideInInspector]
    public bool isLoading = false;
    [HideInInspector]
    public bool isLoadend = true;


    public GameObject UiRoot;

    /// <summary>
    /// 存放加载好的资源
    /// </summary>
    private Dictionary<string, UObject> assetDict = new Dictionary<string, UObject>();


    /// 数据存放目录
    public static string DataPath
    {
        get
        {
            if (dataPath != null)
                return dataPath;
#if UNITY_EDITOR 
            string localPath = @"Assets/StreamingAssets";
#elif UNITY_STANDALONE_WIN
            string localPath = @"MyGame_Data/StreamingAssets";
#elif UNITY_IOS
            string localPath = "Raw";
#elif UNITY_ANDROID         
            string localPath = @"Assets/StreamingAssets";
#endif

            if (Application.isMobilePlatform)
            {
                dataPath = Application.streamingAssetsPath+ "/";
            }
            else
            {
                int i = Application.dataPath.LastIndexOf('/');
                dataPath = Application.dataPath.Substring(0, i + 1)+ localPath + "/";
            }
            return dataPath;
        }
    }

    /// 游戏根目录
    public static string GameRootPath
    {
        get
        {
            if (Application.isMobilePlatform)
            {
                return Application.persistentDataPath + "/";
            }
            else
            {
                int i = Application.dataPath.LastIndexOf('/');
                return Application.dataPath.Substring(0, i + 1);
            }

        }
    }
    

    void Awake()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)//http://answers.unity3d.com/questions/725419/filestream-binaryformatter-from-c-to-ios-doesnt-wo.html
        {
            System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        }
        //永不睡眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //游戏帧率
        Application.targetFrameRate = 45;


        DontDestroyOnLoad(gameObject);


    }

    void Start()
    {
        if (instance == null)
            Initialize();

        UiRoot = GameObject.Find("UiRoot");
    }

    internal void Update()
    {

    }

    /// <summary>
    /// 切换界面场景
    /// </summary>
    /// <returns>The scene I enumerator.</returns>
    /// <param name="name">Name.</param>
    /// <param name="is3D">If set to <c>true</c> is3 d.</param>
    /// <param name="isDelOld">If set to <c>true</c> is del old.</param>
    /// <param name="cb">Cb.</param>
    IEnumerator LoadSceneIEnumerator(string name, bool isDelOld = false, ScenceLoadCb cb = null)
    {
        LoadAsset<GameObject>("UILayer", name, delegate (UObject obj2)
        {
            if (isDelOld)
            {
                for (int i = 0; i < UiRoot.transform.childCount; ++i)
                {
                    GameObject go = UiRoot.transform.GetChild(i).gameObject;
                    //if (go.name != "ChatQp" && go.name != "ChatJy")
                    Destroy(go);
                }
            }
            GameObject gameObj = null;
            if (obj2 != null)
            {
                gameObj = Instantiate(obj2, UiRoot.transform) as GameObject;
                var rtans = gameObj.GetComponent<RectTransform>();
                rtans.offsetMin = Vector2.zero;
                rtans.offsetMax = Vector2.zero;
                gameObj.GetComponent<Transform>().localRotation = Quaternion.Euler(0f, 0f, 0f);
                gameObj.name = name;
            }
            gameObj.GetComponent<Canvas>().worldCamera = UICam;
            var canScaler = gameObj.GetComponent<CanvasScaler>();
            canScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            if (cb != null)
                cb(gameObj);

            Resources.UnloadUnusedAssets();
            isLoadend = true;

            if (loadTask.Count > 0)
            {
                IEnumerator task = loadTask[0];
                loadTask.RemoveAt(0);
                StartCoroutine(task);
            }
        });
        yield break;
    }


    /// <summary>
    /// 切换界面
    /// </summary>
    /// <param name="name">界面名称</param>
    /// <param name="is3D">是否加载相同名称的3D场景</param>
    /// <param name="isDelOld">是否移除旧的界面和场景</param>
    /// <param name="cb">切换完成后执行功能</param>
    public void LoadScene(string name, bool isDelOld = false, ScenceLoadCb cb = null)
    {
        if (!isLoadend)
        {
            loadTask.Add(LoadSceneIEnumerator(name,  isDelOld, cb));
            return;
        }
        isLoadend = false;
        isLoading = true;

        StartCoroutine(LoadSceneIEnumerator(name,  isDelOld, cb));
    }

    List<IEnumerator> loadTask = new List<IEnumerator>();

    GameObject root3D = null;


    public void Root3d(GameObject go) {
        root3D = go;
        root3D.SetActive(false);
    }
    

    /// <summary>
    /// 获取指定场景的层控制器组件
    /// </summary>
    /// <returns>场景的层控制器组件</returns>
    /// <param name="sceneName">场景名字</param>
    /// <typeparam name="T">层控制器类型</typeparam>
    public T GetSceneLayerController<T>(string sceneName) where T : MonoBehaviour, new()
    {
        Transform trans = transform.Find("NormalLayer/" + sceneName);
        if (trans == null)
            return null;
        T layerController = trans.GetComponent<T>();
        return layerController;
    }


    /// 所有AB名称
    string[] m_AllManifest = null;
    /// StreamingAssets.manifest
    AssetBundleManifest m_AssetBundleManifest = null;
    /// 存放每个ab的依赖
    Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();
    /// 存放已经加载的ab
    Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
    /// 存放每个ab的加载资源需求列表
    Dictionary<string, List<LoadAssetRequest>> m_LoadRequests = new Dictionary<string, List<LoadAssetRequest>>();

    class LoadAssetRequest
    {
        /// 资源类型
        public Type assetType;
        /// 资源名称
        public string assetName;
        /// 加载完成回调
        public Action<UObject> sharpFunc;
    }

    


    /// 加载StreamingAssets.manifest文件
    public void Initialize()
    {
        AssetBundle manifestBundle = AssetBundle.LoadFromFile(DataPath + "StreamingAssets");
        if (manifestBundle != null)
        {
            m_AssetBundleManifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
            m_AllManifest = m_AssetBundleManifest.GetAllAssetBundles();
        }
        instance = this;
        //LoadScene("Menu");
    }
    

    /// <summary>
    /// 修正ab路径
    /// </summary>
    /// <returns>The real asset path.</returns>
    /// <param name="abName">Ab name.</param>
    string GetRealAssetPath(string abName)
    {
        abName = abName.ToLower();
        if (!abName.EndsWith(".unity3d"))
        {
            abName += ".unity3d";
        }
        return abName;
        //        if (abName.Contains("/"))
        //        {
        //            return abName;
        //        }
        //        for (int i = 0; i < m_AllManifest.Length; i++)
        //        {
        ////            int index = m_AllManifest[i].LastIndexOf('/');  
        ////            string path = m_AllManifest[i].Remove(0, index + 1);
        //            if (path.Equals(abName))
        //            {
        //                return m_AllManifest[i];
        //            }
        //        }
        //        Debug.LogError("GetRealAssetPath Error:>>" + abName);
        //        return null;
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    public void LoadAsset<T>(string abName, string assetName, Action<UObject> action = null) where T : UObject
    {
        abName = GetRealAssetPath(abName);  //获取ab路径
		if (null == abName ) //20180403
		{
			Debug.LogError ("null == abName ");
			return;
		}
        UObject result = null;
        if (assetDict.TryGetValue(abName + assetName+ typeof(T), out result))
        {
            if (action != null)
            {
                try
                {
                    action(result);    //将加载结果资源传给回调函数
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex.ToString());
                }
            }
            return;
        }

        LoadAssetRequest request = new LoadAssetRequest();  //新建加载资源需求
        request.assetType = typeof(T);
        request.assetName = assetName;
        request.sharpFunc = action;

        List<LoadAssetRequest> requests = null;
        if (!m_LoadRequests.TryGetValue(abName, out requests))  //该ab的加载资源需求列表为空
        {
            requests = new List<LoadAssetRequest>();
            requests.Add(request);
            m_LoadRequests.Add(abName, requests);
            StartCoroutine(OnLoadAsset<T>(abName));     //该ab的第一个需求，开始执行
        }
        else
        {
            requests.Add(request);      //该ab有需求正在执行，直接添加到需求列表
        }
    }


    /// <summary>
    /// 开始加载资源
    /// </summary>
    /// <param name="abName">Ab name.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    IEnumerator OnLoadAsset<T>(string abName) where T : UObject
    {
        AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);
        if (bundleInfo == null)         //ab还没有加载
        {
            yield return StartCoroutine(OnLoadAssetBundle(abName));  //先加载ab

            bundleInfo = GetLoadedAssetBundle(abName);
            if (bundleInfo == null)     //ab没有加载成功
            {
                m_LoadRequests.Remove(abName);  //移除该ab的加载资源需求列表
                Debug.LogError("OnLoadAsset--->>>" + abName);
                yield break;
            }
        }
        List<LoadAssetRequest> list = null;
        if (!m_LoadRequests.TryGetValue(abName, out list)) //获取该ab的加载资源需求列表
        {
            m_LoadRequests.Remove(abName);  //未获取到，移除该ab的加载资源需求列表
            yield break;
        }
        for (int i = 0; i < list.Count; i++)
        {
            string assetName = list[i].assetName;
            AssetBundle ab = bundleInfo.m_AssetBundle;
            string assetPath = assetName;
            AssetBundleRequest request = ab.LoadAssetAsync(assetName, list[i].assetType);   //开始异步加载资源
            yield return request;
            UObject result = request.asset;  //加载完成，存储加载结果资源

            //T assetObj = ab.LoadAsset<T>(assetPath);  //同步加载
            //result = assetObj;

            assetDict[abName + assetName + typeof(T)] = result;

            if (list[i].sharpFunc != null)
            {
                try
                {
                    list[i].sharpFunc(result);    //将加载结果资源传给回调函数
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex.ToString());
                }
                finally
                {
                    list[i].sharpFunc = null;
                }
            }
            bundleInfo.m_ReferencedCount++;     //该ab被引用数量加1
        }
        m_LoadRequests.Remove(abName);      //所有需求执行完成，从字典中移除该ab的加载资源需求列表
    }


    /// <summary>
    /// 加载unity3d文件
    /// </summary>
    /// <param name="abName">Ab name.</param>
    /// <param name="type">Type.</param>
    IEnumerator OnLoadAssetBundle(string abName)
    {
        string url = DataPath + abName;
        string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);   //获取要加载的ab所有的依赖资源
        if (dependencies.Length > 0)    //有依赖资源
        {
            if (m_Dependencies.ContainsKey(abName)) //已经在加载中了
            {
                while (m_LoadedAssetBundles.ContainsKey(abName))
                {
                    yield return null;
                }
                yield break;
            }


            m_Dependencies.Add(abName, dependencies);   //添加该ab所依赖的资源列表到字典
            for (int i = 0; i < dependencies.Length; i++)
            {
                string depName = dependencies[i];
                AssetBundleInfo bundleInfo = null;
                if (m_LoadedAssetBundles.TryGetValue(depName, out bundleInfo))  //依赖资源已经加载了
                {
                    bundleInfo.m_ReferencedCount++;     //该依赖资源的被引用的数量加1
                }
                else if (!m_LoadRequests.ContainsKey(depName))  //依赖资源还未加载
                {
                    yield return StartCoroutine(OnLoadAssetBundle(depName));  //先加载依赖资源
                }
            }
        }

        dependencies = null;

        //WWW download = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(abName), 0);       //加载资源
        //yield return download;
        //AssetBundle assetObj = download.assetBundle;
        AssetBundle assetObj = AssetBundle.LoadFromFile(url);
        if (assetObj != null)
        {
            m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));    //将加载完成的ab添加到字典
        }
        yield break;
    }


    /// <summary>
    /// 获取已经加载完成的ab
    /// </summary>
    /// <returns>The loaded asset bundle.</returns>
    /// <param name="abName">Ab name.</param>
    AssetBundleInfo GetLoadedAssetBundle(string abName)
    {
        AssetBundleInfo bundle = null;
        m_LoadedAssetBundles.TryGetValue(abName, out bundle);
        if (bundle == null)
            return null;

        //该资源没有依赖，只需ab自身
        string[] dependencies = null;
        if (!m_Dependencies.TryGetValue(abName, out dependencies))
            return bundle;

        // 确保所有依赖资源已经都加载了
        foreach (var dependency in dependencies)
        {
            AssetBundleInfo dependentBundle;
            m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
            if (dependentBundle == null)
                return null;
        }
        return bundle;
    }

    /// <summary>
    /// 此函数交给外部卸载专用，自己调整是否需要彻底清除AB
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="isThorough"></param>
    public void UnloadAssetBundle(string abName, bool isThorough = false)
    {
        abName = GetRealAssetPath(abName);
        Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + abName);
        UnloadAssetBundleInternal(abName, isThorough);
        UnloadDependencies(abName, isThorough);
        Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + abName);
    }

    void UnloadDependencies(string abName, bool isThorough)
    {
        string[] dependencies = null;
        if (!m_Dependencies.TryGetValue(abName, out dependencies))  //该ab没有依赖
            return;

        // 卸载所有依赖的ab
        foreach (var dependency in dependencies)
        {
            UnloadAssetBundleInternal(dependency, isThorough);
        }
        m_Dependencies.Remove(abName);
    }

    void UnloadAssetBundleInternal(string abName, bool isThorough)
    {
        AssetBundleInfo bundle = GetLoadedAssetBundle(abName);
        if (bundle == null)     //该ab没有加载
            return;

        if (bundle.m_ReferencedCount <= 0)      //该ab未被引用
        {
            if (m_LoadRequests.ContainsKey(abName))
            {
                return;     //如果当前AB处于Async Loading过程中，卸载会崩溃，只减去引用计数即可
            }
            bundle.m_AssetBundle.Unload(isThorough);
            m_LoadedAssetBundles.Remove(abName);
            Debug.Log(abName + " has been unloaded successfully");
        }
    }
    
    public void DebugLog(string msg)
    {
        Debug.Log(msg);
    }
}





















































