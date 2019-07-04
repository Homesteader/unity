using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class SQSceneLoader : MonoBehaviour {
    public static SQSceneLoader It;

    public delegate void LoadSceneFinish();
    public string curSceneName = "";
    public int curSceneIndex = 0;
    public AsyncOperation mAsync;
    private Action<float> mProcessActCall;
    void Awake()
    {
        It = this;
    }

    void Start()
    {

    }

    /// <summary>
    /// 加载场景 
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="finish"></param>
    public void LoadScene(string sceneName, LoadSceneFinish finish = null)
    {
        //Global.It.mSceneLoadMaskCtr.OpenWindow();
        mAsync = SceneManager.LoadSceneAsync(sceneName);
        curSceneName = sceneName;
        StartCoroutine(WaitLoadScene(mAsync, finish));
    }

    public AsyncOperation LoadScene(int Index, Action<float> processAct, LoadSceneFinish finish = null)
    {
        mAsync = SceneManager.LoadSceneAsync(Index);
        curSceneIndex = Index;
        mProcessActCall = processAct;
        StartCoroutine(WaitLoadScene(mAsync, finish));
        return mAsync;
    }


    IEnumerator WaitLoadScene(AsyncOperation async, LoadSceneFinish finish = null)
    {
		SQDebug.Log (" 加 载。。。="+async.progress);
        yield return async;
		SQDebug.Log (" 加 222 载。。。=="+async.progress);
       
        yield return new WaitForSeconds(1.0f);

		if (finish != null)
		{
			finish();
		}
        //Global.Inst.mSceneLoadMaskCtr.HideWindow();
        SQDebug.Log (" 加 222 载。。end。==");
    }

    //获取当前场景名字
    public string GetCurSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }


	void Update(){
		if (mProcessActCall != null) {
			if (mAsync.progress <= 1) {
				mProcessActCall (mAsync.progress);
				SQDebug.Log (" 加 4444 载。。。==" + mAsync.progress);
				if (mAsync.progress == 1) {
					mProcessActCall = null;
				}
			} 
		}
	}

}
