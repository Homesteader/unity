using UnityEngine;
using System.Collections;

public class NiuniuCoinFlyWidget : BaseViewWidget
{
    public GameObject mCoinItem;//金币item
    public GameObject mPointItem;//积分item
    public GameObject mCoinChild;//金币父物体
    public GameObject mPointChild;//积分父物体


    #region 金币
    /// <summary>
    /// 金币飞行
    /// </summary>
    /// <param name="from">初始位置（世界坐标）</param>
    /// <param name="to">目标位置（世界坐标）</param>
    /// <param name="time">时间</param>
    public void SetCoinFly(Vector3 from, Vector3 to, int num,float time)
    {
        StartCoroutine(InstantiateCoinFly(from,to,num,time));
    }

    /// <summary>
    /// 生成金币
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    private IEnumerator InstantiateCoinFly(Vector3 from, Vector3 to, int num,float time) {
        for (int i=0;i<num;i++) {
            GameObject obj = NGUITools.AddChild(mCoinChild, mCoinItem);
            obj.transform.position = from;
            obj.SetActive(true);
            Move(obj, time, to, 0f);
            yield return new WaitForSeconds(0.1f);

        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="time">移动世界</param>
    /// <param name="to">移动目标位置</param>
    /// <param name="delayTime">延迟时间</param>
    private void Move(GameObject obj, float time, Vector3 to, float delayTime)
    {
        if (delayTime <= 0)
            iTween.MoveTo(obj, iTween.Hash("position", to, "time", time, "islocal", false, "easetype", iTween.EaseType.linear, "oncomplete", "OnMoveFinish", "oncompletetarget", gameObject, "oncompleteparams", obj));
        else
            DelayRun(delayTime, () =>
            {
                iTween.MoveTo(obj, iTween.Hash("position", to, "time", time, "islocal", false, "easetype", iTween.EaseType.linear, "oncomplete", "OnMoveFinish", "oncompletetarget", gameObject, "oncompleteparams", obj));
            });
    }

    /// <summary>
    /// 移动结束隐藏金币
    /// </summary>
    /// <param name="obj"></param>
    private void OnMoveFinish(GameObject obj)
    {
        if (obj != null)
            Destroy(obj);
    }
    #endregion
}
