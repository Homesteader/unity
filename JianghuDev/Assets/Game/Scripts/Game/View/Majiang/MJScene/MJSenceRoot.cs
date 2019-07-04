using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJSenceRoot : MonoBehaviour
{

    public MjPlayerBase mMyself;
    public MjPlayerBase[] mOther;
    public Transform[] mTouzi;//骰子
    public Transform mDesk;//桌子
    public GameObject[] mLight;//该玩家出牌时高亮物体
    public FGrid[] mDeskCardGrid;//桌上的牌
    public GameObject mDescCard;//桌上牌

    public const int mMax = 136;//麻将总数
    public GameObject mChangeThreeAnim;

    public Transform mEffectRoot;//特效父节点
    //public GameObject[] mEffect;
    private Dictionary<string, GameObject> mEffect = new Dictionary<string, GameObject>();//加载出的特效预制体
    private Dictionary<string, Dictionary<int, GameObject>> mEffectDic = new Dictionary<string, Dictionary<int, GameObject>>();//所有特效,根据特效名分类
    private List<GameObject> mDeskCardList = new List<GameObject>();//桌上剩余的牌
    public Vector3[] mEffectPos = new Vector3[] { new Vector3(-0.75f, 0.56f, 0), new Vector3(0, 0, 0), new Vector3(0.75f, 0.56f, 0), new Vector3(0, 1.14f, 0), };//从自己开始算第2个，逆时针算
    public Vector3[] mLightRot = new Vector3[] { new Vector3(0, 90, 0), new Vector3(0, 0, 0), new Vector3(0, 270, 0), new Vector3(0, 180, 0), };//从自己开始算第2个，逆时针算
    public Mesh[] mMeshs;

    public TextMesh mFanShu;//番数
    public TextMesh mRoomId;//房间号
    public TextMesh mPlayNum;//局数
    public TextMesh mBaseScore;//底分

    public GameObject[] mHuEffect;//胡特效
    public GameObject mNormalWin;//普通胡特效

    private const string EFFECT_PATH = "mahjong/FX Prefab/";//特效路径

    void Awake()
    {
    }

    /// <summary>
    /// 重置场景中的物体
    /// </summary>
    public void ResetScene()
    {
        //重置玩家牌
        mMyself.ResetCards();
        for (int i = 0; i < mOther.Length; i++)
            mOther[i].ResetCards();
        //重置桌上的牌
        mDeskCardList.Clear();
        //重置 色子显示
        SetTouZiNum(null);
        for (int i = 0; i < mDeskCardGrid.Length; i++)
        {
            int len = mDeskCardGrid[i].transform.childCount;
            for (int j = 0; j < len; j++)
                mDeskCardGrid[i].transform.GetChild(j).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置房间描述
    /// </summary>
    /// <param name="fanshu">番数</param>
    /// <param name="roomId">房间ID</param>
    /// <param name="curPlayeNum">当前第几局</param>
    /// <param name="allPlayNum">总局数</param>
    /// <param name="baseScore">底分</param>
    public void SetDescInfo(int fanshu, int roomId, int curPlayeNum, int allPlayNum, int baseScore)
    {
        mFanShu.text = fanshu == -1 ? "不封顶" : fanshu + "番封顶";
        if (roomId == 0)
            mRoomId.text = "";
        else
            mRoomId.text = "房间号：" + roomId;
        mPlayNum.text = "局数：" + curPlayeNum + "/" + allPlayNum;
        mBaseScore.text = "底分：" + baseScore;
    }

    /// <summary>
    /// 设置局数
    /// </summary>
    /// <param name="num">当前局数</param>
    /// <param name="allnum">总局数</param>
    public void SetPlayNum(int num, int allnum)
    {
        mPlayNum.text = "局数：" + num + "/" + allnum;
    }

    /// <summary>
    /// 设置桌子的旋转位置
    /// </summary>
    /// <param name="mySeat">我的座位号，1为东，2为南，3为西，4为北</param>
    public void SetDescRotate(int mySeat)
    {
        Quaternion q = Quaternion.Euler(new Vector3(0, 90 * mySeat, 0));
        mDesk.localRotation = q;
    }

    /// <summary>
    /// 设置该谁出牌
    /// </summary>
    /// <param name="seat">座位号</param>
    public void SetLight(int seat)
    {
        for (int i = 0; i < mLight.Length; i++)
            mLight[i].SetActive(false);
        if (seat == 0)
            return;
        int index = MJGameModel.Inst.mSeatToDirectionIndex[seat];
        mLight[index].SetActive(true);
        
    }
    #region 特效显示
    /// <summary>
    /// 设置特效显示
    /// </summary>
    /// <param name="id">特效ID</param>
    /// <param name="seatId">座位号index，转换过后的</param>
    public void SetEffectShow(MJoptInfoData data, int seatId, int[] _type, int subtype)
    {
        if (data.ins == eMJInstructionsType.HU)//胡牌
        {
            if (seatId != data.seatId)//不是主动操作的人
                return;
            //先获取配置表
            MJGameInsEffectConfig con = GetInsEffectConfig(data.ins);
            if (con == null)
                return;

            if ((eHuThrType)data.thrType != eHuThrType.NONE)//三级类型
            {
                LoadAndShowEffect(con.thrTypeEffect[data.thrType], seatId);
            }
            else if ((eHuSubType)data.subType != eHuSubType.NONE)//二级类型
            {
                LoadAndShowEffect(con.subTypeEffect[data.subType], seatId);
            }
            else if ((eHuType)data.type != eHuType.NONE)//一级类型
            {
                LoadAndShowEffect(con.effect[data.type], seatId);
            }
        }
        else if (data.ins == eMJInstructionsType.YPDX || data.ins == eMJInstructionsType.PENG)//一炮多响、碰
        {
            //先获取配置表
            MJGameInsEffectConfig con = GetInsEffectConfig(data.ins);
            if (con == null)
                return;
            if (seatId == data.seatId)//主动操作的人
                LoadAndShowEffect(con.effect[0], seatId, data.ins == eMJInstructionsType.YPDX);
        }
        else if (data.ins == eMJInstructionsType.GANG)//杠
        {
            //先获取配置表
            MJGameInsEffectConfig con = GetInsEffectConfig(data.ins);
            if (con == null)
                return;
            if (data.seatId != seatId)//不是主动操作的人就返回
                return;
            int type = data.type;
            LoadAndShowEffect(con.subTypeEffect[type], seatId);

        }
    }

    /// <summary>
    /// 获取特效配置表
    /// </summary>
    /// <param name="insType">操作类型</param>
    /// <returns></returns>
    private MJGameInsEffectConfig GetInsEffectConfig(eMJInstructionsType insType)
    {
        string index = insType.GetHashCode().ToString();
        ConfigDada con = ConfigManager.GetConfigs<MJGameInsEffectConfig>().Find(o => o.conIndex.Equals(index));
        if (con == null)
            return null;
        return con as MJGameInsEffectConfig;
    }

    /// <summary>
    /// 加载和添加特效
    /// </summary>
    /// <param name="name"></param>
    /// <param name="seadId"></param>
    /// <returns></returns>
    private void LoadAndShowEffect(string name, int seadId, bool isOnly = false)
    {
        Dictionary<int, GameObject> dic = null;
        GameObject obj = null;
        if (mEffectDic.TryGetValue(name, out dic))//如果有这个特效
        {
            if (dic == null)
                dic = new Dictionary<int, GameObject>();
            if (isOnly)//是否是每一个人专属的
            {
                if (dic.ContainsKey(seadId))//如果有这个玩家座位号
                    obj = dic[seadId];
            }
            else
            {
                if (dic.ContainsKey(1))
                    obj = dic[1];
            }
        }
        else
            dic = new Dictionary<int, GameObject>();
        if (obj == null)
        {
            GameObject effect = null;
            if (!mEffect.TryGetValue(name, out effect))
            {
                effect = Assets.LoadPrefab(EFFECT_PATH + name);
                mEffect.Add(name, effect);
            }
            obj = NGUITools.AddChild(mEffectRoot.gameObject, effect);
        }
        if (obj == null)
            return;
        int index = MJGameModel.Inst.mnewSeatToIndex[seadId];
        obj.transform.localPosition = mEffectPos[index];
        if (isOnly)//单独使用
            dic[seadId] = obj;
        else
            dic[1] = obj;
        mEffectDic[name] = dic;
        obj.SetActive(false);
        obj.SetActive(true);
    }

    #endregion
    /// <summary>
    /// 设置 一炮多项 特效
    /// </summary>
    public void SetYPDXEff(MJoptInfoData data)
    {
    }

    /// <summary>
    /// 流局
    /// </summary>
    public void SetNoWinnerShow()
    {
        List<ConfigDada> conList = ConfigManager.GetConfigs<MJGameInsEffectConfig>();
        ConfigDada con = conList.Find(o => o.conIndex == "100");
        if (con == null)
            return;
        MJGameInsEffectConfig insCon = con as MJGameInsEffectConfig;
        LoadAndShowEffect(insCon.effect[0], MJGameModel.Inst.mMySeatId);
    }


    public void SetDeskCard(int allCount, List<int> num, bool isinit)
    {
        int[] n = new int[2];
        if (num == null)
        {
            n[0] = 1; n[1] = 2;
        }
        else
            n = num.ToArray();
        SetDeskCard(allCount, n, isinit);
    }

    /// <summary>
    /// 设置桌面上剩余的牌
    /// </summary>
    /// <param name="allCount">剩余牌数量</param>
    /// <param name="num">骰子点数</param>
    /// <param name="isinit">是否是初始化</param>
    public void SetDeskCard(int allCount, int[] num, bool isinit)
    {
        #region 初始化
        if (isinit)
        {
            int len;
            GameObject obj;
            //以东家为原点，开始被摸牌的玩家index

            int startIndex = (num[0] + num[1] - 1) % 4;

            //以东家第一张牌为原点，开始被摸牌的序号
            int startCard = num[0] < num[1] ? num[0] : num[1];
            startCard = startCard * 2 + startIndex * mMax / 4;
            List<GameObject> list = new List<GameObject>();
            int kkk = 0;
            for (int i = 0; i < mDeskCardGrid.Length; i++)
            {
                len = mDeskCardGrid[i].transform.childCount;
                for (int j = 0; j < mMax / 4; j++)
                {

                    if (j < len)
                        obj = mDeskCardGrid[i].transform.GetChild(j).gameObject;
                    else
                        obj = NGUITools.AddChild(mDeskCardGrid[i].gameObject, mDescCard);
                    list.Add(obj);
                    obj.name = kkk.ToString();
                    kkk++;
                    obj.SetActive(true);
                }
                mDeskCardGrid[i].Reposition();
            }
            for (int i = 0; i < list.Count; i++)
                list[i].SetActive(false);
            int min = startCard;
            int max = mMax - allCount + min;
            int index = max % mMax;
            //把剩余的object添加进list，以后没减少一张牌就把第一张移除
            for (int i = 0; i < allCount; i++)
            {
                mDeskCardList.Add(list[index]);
                list[index].SetActive(true);
                index++;
                index = index % mMax;
            }
        }
        #endregion
        else
        {
            if (mDeskCardList.Count > 0)
            {
                mDeskCardList[0].SetActive(false);
                mDeskCardList.RemoveAt(0);
            }
        }
    }

    /// <summary>
    /// 设置收集的牌里面需要显示遮罩的牌
    /// </summary>
    /// <param name="num">小于0表示隐藏</param>
    public void SetSameCardMaskShow(int num)
    {
        if (mMyself != null)
            mMyself.SetSameCardMaskShow(num);
        for (int i = 0; i < mOther.Length; i++)
        {
            if (mOther[i] != null)
                mOther[i].SetSameCardMaskShow(num);
        }
    }
    /// <summary>
    /// 设置 色子显示
    /// </summary>
    /// <param name="num"> num 为空时还原到1 1</param>
    public void SetTouZiNum(List<int> num = null)
    {
        if (num == null)
        {
            for (int i = 0; i < mTouzi.Length; i++)
            {
                Vector3 agule = mTouzi[i].localRotation.eulerAngles; ;
                float a = 0f;
                Vector3 ver = new Vector3(1, 0, 0);
                if (agule.x != 0)
                {
                    a = agule.x;
                }
                else if (agule.y != 0)
                {
                    a = agule.y;
                }
                else if (agule.z != 0)
                {
                    a = agule.z;
                    ver = new Vector3(0, 0, 1);
                }
                mTouzi[i].Rotate(ver, -a);
            }
            return;
        }
        for (int i = 0; i < mTouzi.Length; i++)
        {
            Vector3 aixs = new Vector3(1f, 0f, 0f);
            float angule = 0f;
            switch (num[i])
            {
                case 1:
                    aixs = new Vector3(1f, 0f, 0f);
                    angule = 0f;
                    break;
                case 2:
                    aixs = new Vector3(1f, 0f, 0f);
                    angule = 90f;
                    break;
                case 3:
                    aixs = new Vector3(0f, 0f, 1f);
                    angule = -90f;
                    break;
                case 4:
                    aixs = new Vector3(1f, 0f, 0f);
                    angule = -90f;
                    break;
                case 5:
                    aixs = new Vector3(0f, 0f, 1f);
                    angule = 90f;
                    break;
                case 6:
                    aixs = new Vector3(1f, 0f, 0f);
                    angule = 180f;
                    break;
            }
            mTouzi[i].Rotate(aixs, angule);
        }
    }
}
