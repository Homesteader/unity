using UnityEngine;
using System.Collections;
using System;

public class BaseController : IDisposable
{
    protected string mViewName;
    protected BaseView mBaseView;
    protected BaseModel mBaseModel;
    protected string mViewPath;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name">view类名</param>
    /// <param name="path">view预制体路径</param>
    public BaseController(string name, string path)
    {
        mViewName = name;
        mViewPath = path;
    }

    public virtual BaseView OpenWindow()
    {
        if (mBaseView != null)
        {
            mBaseView.gameObject.SetActive(true);
            return mBaseView;
        }
        if (string.IsNullOrEmpty(mViewPath))
        {
            SQDebug.Log("view path is null");
            return null;
        }

        if (string.IsNullOrEmpty(mViewName))
        {
            SQDebug.Log("view name is null");
            return null;
        }

        GameObject obj = GameObject.Instantiate((Resources.Load(mViewPath))) as GameObject;
     
        if (obj == null)
        {
            SQDebug.Log("加载错误：" + mViewPath);
            return null;
        }
        obj.transform.parent = Global.Inst.mUIRoot;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj.transform.localScale = Vector3.one;
        obj.gameObject.SetActive(true);
        mBaseView = obj.GetComponent<BaseView>();        
        mBaseView.AddView(mViewName);
      
        return mBaseView;
    }

    public void CloseWindow()
    {
        if (mBaseView != null)
            mBaseView.Close();
    }

    public void HideWindow()
    {
        if (mBaseView != null)
            mBaseView.gameObject.SetActive(false);
    }

    public void SetModel<T>() where T : new()
    {
        mBaseModel = new T() as BaseModel;
        mBaseModel.SetController(this);
    }

    public virtual void Dispose()
    {
        if (mBaseView != null)
            mBaseView.Dispose();
        if (mBaseModel != null)
            mBaseModel.Dispose();
    }
}
