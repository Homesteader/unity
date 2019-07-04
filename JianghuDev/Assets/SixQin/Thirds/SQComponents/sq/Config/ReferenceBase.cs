using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReferenceBase<T>
{
    public Dictionary<int, T> mDic = new Dictionary<int, T>();
    //
    public List<T> data = new List<T>();

    /// <summary>
    /// 得到某一个
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public T GetReferenceById(int id)
    {
        T t;
        if (mDic.TryGetValue(id, out t))
        {
            return t;
        }

        return t;
    }


    public List<T> GetList()
    {
        return data;
    }


}


   

