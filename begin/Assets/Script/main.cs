using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SerializableClass
{
    public string name;
    public int age;
    public bool isHero;

    public SerializableClass(string name, int age, bool isHero)
    {
        this.name = name;
        this.age = age;
        this.isHero = isHero;
    }
}

public class main : MonoBehaviour
{
    public SerializableClass serializableClass;
    
    private void Start()
    {
        this.serializableClass = new SerializableClass("华夏", 20, false);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10,10,150,100), "Serialize"))
        {
            string fileName = "Assets/File/serializableClass.bat";
            Stream fStream = new FileStream(fileName, FileMode.Create,FileAccess.ReadWrite);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fStream,this.serializableClass);
            this.serializableClass.name = "五千年";
            fStream.Close();
            Debug.Log("name is: " + this.serializableClass.name);
        }

        if (GUI.Button(new Rect(300,10,150,100), "DeSerialize"))
        {
            string fileName = "Assets/File/serializableClass.bat";
            Stream fStream = new FileStream(fileName, FileMode.Open,FileAccess.Read);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            this.serializableClass = binaryFormatter.Deserialize(fStream) as SerializableClass;
            fStream.Close();
            Debug.Log("after name is: " + this.serializableClass.name);
        }
    }
}
