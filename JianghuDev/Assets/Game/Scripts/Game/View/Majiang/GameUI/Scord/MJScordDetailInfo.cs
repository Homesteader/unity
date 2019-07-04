using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MJScordDetailInfo
{
    public Opt[] optList;
    public ResultInfo[] resultInfo;
    public RoomInfo[] roomInfo;
    public StartInfo startInfo;
    public UsersInfo[] usersInfo;


    #region 内部类

    public class Opt
    {
        public List<int> card;//打出的牌
        public int currTurnSeatId;//谁打出的牌
        public eMJInstructionsType ins;
        public int seatId;//座位号
        public int otherSeatId;//被操作的玩家
    }

    public class ResultInfo
    {
        public int[] des;//???
        public string detail;//详细的描述信息
        public int fanshu;//番数
        public int huDePai;//胡牌数
        public Gang[] gang;//杠
        public Chi[] chi;//吃
        public int[] peng;//碰
        public Liang[] liang;//亮的牌
        public bool isZhuang;//是否是庄家
        public string nickName;//玩家昵称
        public int score;//玩家输赢的分
        public int seatId;//座位号
        public int[] shoupai;//剩余手牌
        public string userId;//用户id

        public class Gang
        {
            public int[] card;//杠的牌
            public int type;//杠的类型
            public int[] seatId;
        }

        public class Chi{
            public int[] card;
            public int[] seatId;
        }

        public class Liang {
            public int[] card;
        }
    }

    public class RoomInfo
    {
        public CreateData createData;//房间玩法信息
        public int curGameNum;//当前游戏局数
        public int mgrSeatId;//？？？
        public int roomId;//房间号

        public class CreateData
        {
            public int maxMul;
            public int maxPlayer;//最大的玩家数量
            public int modelId;
            public int roommode;
            public int paymode;
            public int[] member;
            public int memNum;
            public int maxturn;
            public int isFree;
            public int fangzhu;
            public int baseScore;
            public List<int> rule;//规则
        }
    }

    public class StartInfo
    {
        public int cardCount;//??
        public CardsInfo[] cardsInfo;
        public int currTurnSeatId;//???
        public int[] touzi = new int[] { 1, 2 };
        public class CardsInfo
        {
            public List<int> cards;//牌
            public int currCard;//当前牌
            public int piao;//飘
            public int seatId;//座位号
        }
    }

    public class UsersInfo
    {
        public string headUrl;//头像地址
        public string nickName;//玩家昵称
        public int score;//玩家输赢分数
        public int seatId;//玩家座位号
    }

    #endregion
}

