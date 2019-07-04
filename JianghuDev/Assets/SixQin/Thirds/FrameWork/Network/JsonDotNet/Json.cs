using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System;

public class Json
{
    public static bool isShowLog = true;
    public static object DeserializByte(Type t,byte[] data,int index,int readCount,bool needShowLog = false)
    {
        bool needDecrypt = false;

        string jsonString = Encoding.UTF8.GetString(data,index,readCount);
        if (jsonString == default(string) || jsonString == "") return null;
        if (isShowLog || needShowLog)
        {
             SQDebug.LogWarning("Json: " + jsonString);
            int i = 0;
        }
        if (needDecrypt) jsonString = ResourcesEncryption.AES_Decrypt(jsonString);
        try
        {
            return JsonConvert.DeserializeObject(jsonString,t);
        }
        catch (Exception e)
        {
            SQDebug.Log(e.Message);
            return null;
        }
    }

    public static byte[] SerializerByte(object o)
    {
        string jsonStr = JsonConvert.SerializeObject(o);
        if (isShowLog)
        {
            SQDebug.LogWarning("Json: " + jsonStr);
        }
        if(!string.IsNullOrEmpty(jsonStr))
        {
            return Encoding.UTF8.GetBytes(jsonStr);
        }
        else
        {
            return new byte[0];
        }
    }

    public static T Deserialize<T>(string jsonString,bool needDecrypt = false)
	{
		if (jsonString == default(string) || jsonString == "") return default(T);
        if (isShowLog)
        {
            SQDebug.LogWarning("Json: " + jsonString);
        }
        if (needDecrypt) jsonString = ResourcesEncryption.AES_Decrypt (jsonString);
		try
		{
			return JsonConvert.DeserializeObject<T>(jsonString);
		}
		catch(Exception e)
		{
			SQDebug.Log(e.Message);
			return default(T);
		}
	}

    public static string Serializer<T>(T jsonData, bool needEncrypt = false)
    {
        if (jsonData == null) return default(string);
        string jsonString = "";
        try
        {
            jsonString = JsonConvert.SerializeObject(jsonData);
            if (isShowLog)
            {
                //SQDebug.LogWarning("Json: " + jsonString);
            }
        }
        catch (Exception e)
        {
            SQDebug.Log(e.Message);
            return default(string);
        }

        if (needEncrypt) jsonString = ResourcesEncryption.AES_Encrypt(jsonString);
        return jsonString;
    }

    public static T ReadJsonFile <T>(string path,bool needDecrypt = false)
	{
		return Deserialize<T>(ReadFile(path),needDecrypt);
	}

    private static string ReadFile(string path)
    {

        return Resources.Load<TextAsset>(path).text;
    }

    private static bool FindFile(string path)
    {
        string directoryPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryPath)) return false;
        return File.Exists(path);
    }
}
