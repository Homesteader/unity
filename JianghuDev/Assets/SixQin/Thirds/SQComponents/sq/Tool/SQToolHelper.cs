using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

public class SQToolHelper
{

    #region Base64

    /// <summary>
    /// 编码base64
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Base64Encode(string str)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    }

    /// <summary>
    /// 解base64编码
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Base64Decode(string str)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(str));
    }


    #endregion



    #region 域名ToIP地址

    public static string DoGetHostAddresses(string hostname)
    {
        IPAddress[] ips=null;
        try
        {
            ips = Dns.GetHostAddresses(hostname);
        }
        catch (Exception e)
        {
            SQDebug.Log(e.ToString() );

        }
       

        //Console.WriteLine("GetHostAddresses({0}) returns:", hostname);  
        if (ips!=null)
        {
            SQDebug.Log("ip==" + ips[0].ToString());

            return ips[0].ToString();
        }
        return null;
    }

    #endregion



    #region Object2byte

    ///<summary> 
    /// 序列化 
    /// </summary> 
    /// <param name="data">要序列化的对象</param> 
    /// <returns>返回存放序列化后的数据缓冲区</returns> 
    public static byte[] Serialize(object data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream rems = new MemoryStream();
        formatter.Serialize(rems, data);
        return rems.GetBuffer();
    }

    /// <summary> 
    /// 反序列化 
    /// </summary> 
    /// <param name="data">数据缓冲区</param> 
    /// <returns>对象</returns> 
    public static object Deserialize(byte[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream rems = new MemoryStream(data);
        data = null;
        return formatter.Deserialize(rems);
    }

    #endregion

}
