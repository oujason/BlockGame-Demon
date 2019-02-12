using System.Collections;
using System.Collections.Generic;
using SocketIOClient;
using System;
using SocketIOClient.Messages;
using System.IO;

public class SocketClient{

    private static SocketClient instance;
    public static SocketClient Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new SocketClient();
            }
            return instance;
        }
    }

    Client client;
    List<byte> receiveCache = new List<byte>();

    public void Initialized(string url)
    {
        client = new Client(url);
        client.Opened += SocketOpened;
        client.Message += SocketMessage;
        client.SocketConnectionClosed += SocketConnectionClosed;
        client.Error += SocketError;
        client.Connect();
    }

    public void Send(IMessage msg)
    {
        client.Send(msg);
    }

    public void Close(IMessage msg)
    {
        client.Close();
    }

    private void SocketOpened(object sender,EventArgs e)
    {
        //invoke when socket opened
        
    }

    private void SocketConnectionClosed(object sender, EventArgs e)
    {
        //invoke when socket closed
    }

    private void SocketMessage(object sender, MessageEventArgs e)
    {
        if (e != null && e.Message.Event == "message")
        {
            string msg = e.Message.MessageText;
            GameEnvironment.GetInstance.DebugLog(msg);
        }
    }

    private void SocketError(object sender, SocketIOClient.ErrorEventArgs e)
    {
        if (e != null)
        {
            string msg = e.Exception.Message;
            GameEnvironment.GetInstance.DebugLog(msg);
        }
    }
}

/// <summary>
/// 编码和解码
/// </summary>
public class NetCode
{
    /// <summary>
    /// 将数据编码 消息id+消息长度+消息内容
    /// </summary>
    /// <param name="protoId"></param>
    /// <param name="proto"></param>
    /// <returns></returns>
    public static byte[] Encode(object proto)
    {
        Type type = proto.GetType();
        int protoId = ProtoDic.GetProtoIdByProtoType(type);
        MemoryStream ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize(ms, proto);
        byte[] data = ms.ToArray();
        ms.Dispose();
        ms = new MemoryStream();
        //整形占四个字节，所以声明一个+4+4+消息长度的数组
        byte[] result = new byte[data.Length + 8];
        //使用流将编码写二进制 
        BinaryWriter br = new BinaryWriter(ms);

        br.Write(protoId);
        br.Write(data.Length);
        br.Write(data);
        //将流中的内容复制到数组中
        Buffer.BlockCopy(ms.ToArray(), 0, result, 0, (int)ms.Length);
        br.Close();
        ms.Close();
        //Debug.LogWarning("编码消息id " + protoId + " 长度 " + result.Length + " 内容 " + GetDesc(result));
        return result;
    }

    /// <summary>
    /// 将数据解码
    /// </summary>
    /// <param name="cache">消息队列</param>
    public static byte[] Decode(ref int protoId, ref List<byte> cache)
    {
        //首先要获取消息长度，整形4个字节，如果字节数不足4个字节
        if (cache.Count < 4)
        {
            return null;
        }
        //读取数据
        MemoryStream ms = new MemoryStream(cache.ToArray());
        BinaryReader br = new BinaryReader(ms);
        protoId = br.ReadInt32();
        //消息长度
        int len = br.ReadInt32();
        //根据长度，判断内容是否传递完毕
        if (len > ms.Length - ms.Position)
        {
            return null;
        }
        //获取数据
        byte[] result = br.ReadBytes(len);
        //Debug.LogWarning("解码消息id " + protoId + " 长度 " + len + " 内容 " + GetDesc(result));
        //清空消息池
        cache.Clear();
        //将剩余没处理的消息存入消息池
        cache.AddRange(br.ReadBytes((int)ms.Length - (int)ms.Position));

        return result;
    }

    public static string GetDesc(byte[] bytes)
    {
        string str = "";
        if (bytes == null)
            return str;
        for (int i = 0; i < bytes.Length; i++)
        {
            int b = (int)bytes[i];
            str += b.ToString() + " ";
        }
        return str;
    }
}
