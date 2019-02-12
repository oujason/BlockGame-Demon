using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;




public class UBinder : SerializationBinder
{
    
    public override Type BindToType(string assemblyName, string typeName)
    {
        var ass = System.Reflection.Assembly.GetExecutingAssembly();
        return ass.GetType(typeName);
    }
}



/// <summary>
/// 公用方法
/// </summary>
public static class GlobalMethod
{
    

    /// <summary>
    /// unicode转成多字节字符串
    /// </summary>
    /// <returns>多字节字符串</returns>
    /// <param name="str">unicode字符串</param>
    public static string UnicodeToMutilBytesString(string str)
    {
        Encoding unicode, gbk;

        unicode = Encoding.GetEncoding("Unicode");
        gbk = Encoding.GetEncoding("GB2312");

        Byte[] buff;
        buff = unicode.GetBytes(str);
        buff = Encoding.Convert(unicode, gbk, buff);

        return gbk.GetString(buff);
    }

    /// <summary>
    /// utf8转成多字节
    /// </summary>
    /// <returns>多字节</returns>
    /// <param name="str">utf8字符串</param>
    public static Byte[] Utf8ToMutilBytes(string str)
    {
        Encoding UTF8, gbk;

        UTF8 = Encoding.GetEncoding("UTF-8");
        gbk = Encoding.GetEncoding("GB2312");

        Byte[] buff;
        buff = UTF8.GetBytes(str);
        buff = Encoding.Convert(UTF8, gbk, buff);

        return buff;
    }

    /// <summary>
    /// utf8转成多字节字符串
    /// </summary>
    /// <returns>多字节字符串</returns>
    /// <param name="str">utf8字符串</param>
    public static string Utf8ToMutilBytesString(string str)
    {
        Encoding UTF8, gbk;

        UTF8 = Encoding.GetEncoding("UTF-8");
        gbk = Encoding.GetEncoding("GB2312");

        Byte[] buff;
        buff = UTF8.GetBytes(str);
        buff = Encoding.Convert(UTF8, gbk, buff);

        return gbk.GetString(buff);
    }

    /// <summary>
    /// 多字节转成utf8字符串
    /// </summary>
    /// <returns>utf8字符串</returns>
    /// <param name="buff">多字节</param>
    public static Byte[] MutilBytesToUtf8(Byte[] buff)
    {
        Encoding UTF8, gbk;

        UTF8 = Encoding.GetEncoding("UTF-8");
        gbk = Encoding.GetEncoding("GB2312");

        Byte[] newbuff = Encoding.Convert(gbk, UTF8, buff);

        return newbuff;
    }


    /// <summary>
    /// 多字节转成utf8字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string MutilBytesToUtf8String(string str)
    {
        Encoding UTF8, gbk;

        UTF8 = Encoding.GetEncoding("UTF-8");
        gbk = Encoding.GetEncoding("GB2312");

        Byte[] buff;
        buff = gbk.GetBytes(str);
        buff = Encoding.Convert(gbk, UTF8, buff);

        return UTF8.GetString(buff);
    }

    /// <summary>
    /// 多字节转Unicode
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Byte[] MutilBytesToUnicode(string str)
    {
        Encoding unicode, gbk;

        unicode = Encoding.GetEncoding("Unicode");
        gbk = Encoding.GetEncoding("GB2312");

        Byte[] buff;
        buff = unicode.GetBytes(str);
        buff = Encoding.Convert(gbk, unicode, buff);

        return buff;
    }

    /// <summary>
    /// 多字节转Unicode
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string MutilBytesToUnicodeString(string str)
    {
        Encoding unicode, gbk;

        unicode = Encoding.GetEncoding("Unicode");
        gbk = Encoding.GetEncoding("GB2312");

        Byte[] buff;
        buff = unicode.GetBytes(str);
        buff = Encoding.Convert(gbk, unicode, buff);

        return gbk.GetString(buff);
    }


    /// <summary>
    /// utf8转unicode
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Utf8ToUnicodeString(string str)
    {
        Encoding UTF8 = Encoding.UTF8;
        Encoding Unicode = Encoding.Unicode;
        Byte[] buff = UTF8.GetBytes(str);
        buff = Encoding.Convert(UTF8, Unicode, buff);
        return Unicode.GetString(buff);
    }


    /// <summary>
    /// 流填充0
    /// </summary>
    /// <param name="br">填充的二进制流</param>
    /// <param name="num">填充位数,默认为1</param>
    public static void DataFillZero(ref BinaryWriter br, int num = 1)
    {
        for (int n = 0; n < num; ++n)
        {
            br.Write((byte)0);
        }
    }

    /// <summary>
    /// 流填充0
    /// </summary>
    /// <param name="br">填充的二进制流</param>
    /// <param name="num">填充位数,默认为1</param>
    public static void DataFillZero(BinaryWriter br, int num = 1)
    {
        DataFillZero(ref br, num);
    }

    /// <summary>
    /// 流读取多余0
    /// </summary>
    /// <param name="br">读取的二进制流</param>
    /// <param name="num">读取位数,默认为1</param>
    public static void DataFilterZero(ref BinaryReader br, int num = 1)
    {
        br.ReadBytes(num);
    }

    /// <summary>
    /// 流读取多余0
    /// </summary>
    /// <param name="br">读取的二进制流</param>
    /// <param name="num">读取位数,默认为1</param>
    public static void DataFilterZero(BinaryReader br, int num = 1)
    {
        DataFilterZero(ref br, num);
    }


    /// <summary>
    /// 得到设备唯一标识符
    /// </summary>
    /// <returns></returns>
    public static string DeviceIdentifier()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    /// <summary>
    /// 小文件读取
    /// </summary>
    public static byte[] ReadSmallFile(string filePath)
    {
        byte[] fileData = null;

        try
        {
            string path = null;
            #if UNITY_EDITOR
            if (UnityEditor.EditorBuildSettings.scenes[1].enabled)
                path = Application.dataPath + "/Original/" + filePath;
             else
			#endif
                path = GameEnvironment.DataPath + filePath;
            FileStream file = new FileStream(path, FileMode.Open);
            if (file.CanRead)
            {
                long nlen = file.Length;
                file.Seek(0, SeekOrigin.Begin);

                fileData = new byte[nlen];

                file.Read(fileData, 0, (int)nlen);
            }

            file.Close();
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message.ToString());
        }

        return fileData;
    }

    /// <summary>
    /// Utf8平台适配字符串编码转换函数
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string PlatformUtf8ToSystem(string str)
    {
#if UNITY_STANDALONE_WIN
        return Utf8ToUnicodeString(str);
#else
        return Utf8ToUnicodeString(str);
#endif
    }

    /// <summary>
    /// GBK平台适配字符串编码转换函数
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string PlatformGBkToSystem(string str)
    {
#if UNITY_STANDALONE_WIN
        return MutilBytesToUnicodeString(str);
#else
        return MutilBytesToUtf8String(str);
#endif
    }

    /// <summary>
    /// 平台适配字符串编码转换函数
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string PlatformSystemToUtf8(string str)
    {
#if UNITY_STANDALONE_WIN
        return MutilBytesToUtf8String(str);
#else
        return str;
#endif
    }

    static void ReadBytes(ref byte[] to , BinaryReader br , int nLen )
    {
        byte[] arr = br.ReadBytes(nLen);
        to = new byte[nLen];
        arr.CopyTo(to, 0);
    }
   
    /// <summary>
    /// 列表深复制
    /// </summary>
    /// <param name="List">需要深复制的列表</param>
    /// <typeparam name="T">类型</typeparam>
    public static List<T> Clone<T>(object List)
    {
        using (Stream objectStream = new MemoryStream())
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(objectStream, List);
            objectStream.Seek(0, SeekOrigin.Begin);
            return formatter.Deserialize(objectStream) as List<T>;
        }
    }

    /// <summary>
    /// 列表复制
    /// </summary>
    /// <param name="List">需要深复制的列表</param>
    /// <typeparam name="T">类型</typeparam>
    public static void Clone<T>(object source, object dest)
    {
        (dest as List<T>).Clear();
        foreach (T item in (source as List<T>))
        {
            (dest as List<T>).Add(item);
        }
    }

    /// <summary>
    /// 字符串转换为int
    /// </summary>
    /// <returns>The to int.</returns>
    /// <param name="Expression">Expression.</param>
    /// <param name="defValue">Def value.</param>
    public static int StrToInt(object Expression, int defValue)
    {
        int result = defValue;
        if (Expression != null)
        {
            if (!int.TryParse(Expression.ToString(), out result))
                result = defValue;
        }
        return result;
    }


   

    public static string GetEncodingGBK(byte[] strConvert, int index, int count)
    {
        return System.Text.Encoding.GetEncoding("gb2312").GetString(strConvert, index, count);
    }

    /// <summary>
    /// 转换为剩余时间格式(xx:xx)字符串
    /// </summary>
    /// <returns>剩余时间格式(xx:xx)字符串</returns>
    /// <param name="leftTime">剩余时间数(秒)</param>
    public static string TransToLeftTimeFormatString(ushort leftTime)
    {
        ushort oneMinuteSeconds = 60;
        ushort mins = (ushort)(leftTime / oneMinuteSeconds);
        ushort seconds = (ushort)(leftTime - oneMinuteSeconds * mins);
        string retVal = string.Format("{0}:{1}", mins.ToString().PadLeft(2, '0'), seconds.ToString().PadLeft(2, '0'));
        return retVal;
    }

    /// <summary>
    /// 转换为剩余时间格式(xx分xx秒)字符串
    /// </summary>
    /// <returns>剩余时间格式(xx:xx)字符串</returns>
    /// <param name="leftTime">剩余时间数(秒)</param>
    public static string TransToLeftTimeFormatStringTts(ushort leftTime)
    {
        ushort oneMinuteSeconds = 60;
        ushort mins = (ushort)(leftTime / oneMinuteSeconds);
        ushort seconds = (ushort)(leftTime - oneMinuteSeconds * mins);
        string retVal = string.Format("{0}分{1}秒", mins.ToString().PadLeft(2, '0'), seconds.ToString().PadLeft(2, '0'));
        return retVal;
    }

    /// <summary>
    /// 转换为剩余时间格式(xx:xx:xx)字符串
    /// </summary>
    /// <returns>剩余时间格式(xx:xx:xx)字符串</returns>
    /// <param name="leftTime">剩余时间数(秒)</param>
    public static string TransToLeftTimeFormatString2(uint leftTime)
    {
        uint oneMinuteSeconds = 60;
        uint oneHourMinutes = 60;
        uint curLeftTime = leftTime;
        uint hours = (uint)(curLeftTime / (oneMinuteSeconds * oneHourMinutes));
        curLeftTime -= hours * (oneMinuteSeconds * oneHourMinutes);
        uint mins = (uint)(curLeftTime / oneMinuteSeconds);
        curLeftTime -= mins * oneMinuteSeconds;
        uint seconds = curLeftTime;
        string retVal = string.Format("{0}:{1}:{2}", hours.ToString().PadLeft(2, '0'), mins.ToString().PadLeft(2, '0'), seconds.ToString().PadLeft(2, '0'));
        return retVal;
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static object Deserialize(byte[] data)
    {
        object obj = null;
        using (MemoryStream mfs = new MemoryStream())
        {
            IFormatter decode = new BinaryFormatter();
            decode.Binder = new UBinder();

            mfs.Write(data, 0, data.Length);
            mfs.Position = 0;


            try
            {
                obj = decode.Deserialize(mfs);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        return obj;
    }


    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="ms"></param>
    /// <returns></returns>
    public static object Deserialize(MemoryStream ms)
    {
        IFormatter decode = new BinaryFormatter();
        decode.Binder = new UBinder();

        return decode.Deserialize(ms);
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static byte[] Serialize(object obj)
    {
        using (MemoryStream mfs = new MemoryStream())
        {
            IFormatter encode = new BinaryFormatter();
            //encode.Binder = new UBinder();
            encode.Serialize(mfs, obj);

            return mfs.ToArray();
        }
    }


    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string Md5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

    /// <summary>
    /// 从包含颜色代码的富文本中踢出颜色代码，用于TTS朗读
    /// </summary>
    /// <returns>The rich text string.</returns>
    /// <param name="text">Text.</param>
    public static string GetRichTextString(string text)
    {
        string result = text;
        if (result.Contains("</color>"))
        {
            result = result.Replace("</color>", "");
            string pattern = "<color=.*?>";
            foreach (Match match in Regex.Matches(result, pattern))
            {
                result = result.Replace(match.Value, "");
            }
        }
        return result.Replace('\n','。');
    }

    /// <summary>
    /// TTS符号替换处理
    /// </summary>
    /// <returns>The percent.</returns>
    /// <param name="text">Text.</param>
    public static string ReplaceSymbol(string text)
    {
        string result = text;
        result = result.Replace('\n', '、').Replace('(', '、').Replace(')', '、').Replace('+', '加').Replace('*', '乘').Replace('：', '、');
        while (result.Contains("%"))
        {
            int index = result.IndexOf('%');
            result = result.Remove(index,1);
            for (int i = index - 1; i >= 0; --i)
            {
                int ascii = (int)result[i];
                if (ascii < 46 || ascii > 57 || ascii == 47)
                {
                    result = result.Insert(i + 1, "百分之");
                    break;
                }
                if (i == 0)
                {
                    result = result.Insert(0, "百分之");
                }
            }
        }

        for (int i = 0; i < result.Length; i++)
        {
            
        }

        return result;
    }

    /// <summary>
    /// UTF8转换成GB2312
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string utf8_gb2312(string text)
    {
        //声明字符集   
        System.Text.Encoding utf8, gb2312;
        //utf8   
        utf8 = System.Text.Encoding.GetEncoding("utf-8");
        //gb2312   
        gb2312 = System.Text.Encoding.GetEncoding("gb2312");
        byte[] utf;
        utf = utf8.GetBytes(text);
        utf = System.Text.Encoding.Convert(utf8, gb2312, utf);
        //返回转换后的字符   
        return gb2312.GetString(utf);
    }

    /// <summary>
    /// 输入框输入字符有效性验证——正整数，只能输入数字
    /// </summary>
    /// <returns>The input positive integer.</returns>
    /// <param name="text">Text.</param>
    /// <param name="charIndex">Char index.</param>
    /// <param name="addedChar">Added char.</param>
    public static char ValidateInputPositiveInteger(string text, int charIndex, char addedChar)
    {
        if ((int)addedChar > 47 && (int)addedChar < 58)
            return addedChar;
        else
            return '\0';
    }


    /// <summary>
    /// 耗时 
    /// </summary>
    /// <param name="func"></param>
    public static void TimeConsumeRun(Action func, string content = "")
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        try
        {
            func();
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("TimeConsumeRun Error:{0} \n {1}", ex.Message, ex.StackTrace));
        }
        finally
        {
            watch.Stop();

            if (content.Length == 0)
                Debug.Log(string.Format("{0} TimeConsumeRun cost time: {1} ms", DateTime.Now.ToString("HH:mm:ss"), watch.ElapsedMilliseconds));
            else
                Debug.Log(string.Format("{0} [{1}] TimeConsumeRun cost time: {2} ms", DateTime.Now.ToString("HH:mm:ss"), content, watch.ElapsedMilliseconds));
        }
    }

	/// <summary>
	/// 从服务器复制来的	20180913
	/// </summary>
	internal class ColdDownTime
	{
		public ColdDownTime(double dbSec)
		{
			m_dbCdSecs = dbSec;
		}
		public bool CheckCdOk()
		{
			var timeNow = DateTime.Now;
			if (m_dbCdSecs > (timeNow - m_timeLast).TotalSeconds)
				return false;

			m_timeLast = timeNow;
			return true;
		}
		double m_dbCdSecs = 0;
		DateTime m_timeLast = DateTime.Now;
	}


    #if UNITY_EDITOR

    static Dictionary<string, System.Diagnostics.Stopwatch> watchDic = new Dictionary<string, System.Diagnostics.Stopwatch>();

    /// <summary>
    /// 开始计时
    /// </summary>
    public static void StartWatch(string watchName)
    {
        if (!watchDic.ContainsKey(watchName))
            watchDic.Add(watchName, new System.Diagnostics.Stopwatch());
        watchDic[watchName].Reset();
        watchDic[watchName].Start();
    }

    /// <summary>
    /// 停止计时
    /// </summary>
    public static void StopWatch(string watchName, long warningMillseconds = 50, string log = null)
    {
        if (!watchDic.ContainsKey(watchName))
            return;
        watchDic[watchName].Stop();
        long time = watchDic[watchName].ElapsedMilliseconds;
        if (time > warningMillseconds)
            Debug.LogWarningFormat("{0}耗时<color=red>{1}</color>毫秒 {2}", watchName, time, log);
        else
            Debug.LogFormat("{0}耗时<color=green>{1}</color>毫秒 {2}", watchName, time, log);
    }



    static Dictionary<string, UnityEngine.Profiling.CustomSampler> profileDic = new Dictionary<string, UnityEngine.Profiling.CustomSampler>();

    /// <summary>
    /// 开始分析
    /// </summary>
    public static void StartProfile(string profileName)
    {
        if (!profileDic.ContainsKey(profileName))
            profileDic.Add(profileName, UnityEngine.Profiling.CustomSampler.Create(profileName));
        profileDic[profileName].Begin();
    }

    /// <summary>
    /// 停止分析
    /// </summary>
    public static void StopProfile(string profileName)
    {
        if (!profileDic.ContainsKey(profileName))
            return;
        profileDic[profileName].End();
    }

    #endif
}