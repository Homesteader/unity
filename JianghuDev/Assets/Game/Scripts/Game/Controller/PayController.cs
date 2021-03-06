﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Net.NetworkInformation;
using System.Net.Sockets;
public class PayController : BaseController
{
    private PayView mPayView;

    private string desc;
    private string v;
    public PayController() : base("PayView", "Windows/PayView/PayView")
    {

    }

    public override BaseView OpenWindow()
    {
        BaseView view = base.OpenWindow();
        if (mPayView == null)
            mPayView = view as PayView;
        return view;
    }
    /// <summary>
    /// 购买商店物品
    /// </summary>
    /// <param name="mData"></param>
    /// <param name="_type"></param>
    public void SendPayInfo(RechargeData mData, int _payType)
    {
        string appName = Application.productName;
        if (mData.type == ePropType.golds)
        {
            desc = mData.cost + "金币";
            v = "金币";
        }
        else
        {
            desc = mData.cost + "房卡";
            v = "房卡";
        }

        string ip = GetIP(ADDRESSFAM.IPv4);//ip地址
        string appInfo = "小熊字牌|com.xftd.xiaoxiongzipai";
#if UNITY_IPHONE
        appInfo = "小熊游戏|com.xftd.xiaoxiongzipai";
#endif
        PayItemId req = new PayItemId();
        req.id = mData.id;
        req.subject = v;
        req.body = desc;
        req.appinfo = appInfo;
        //支付方式
        req.payChannelId = _payType == 1 ? "wechat" : "alipay";
#if UNITY_ANDROID
        req.version = "android_v2_0_1";
#elif UNITY_IPHONE
        req.version = "ios_v2_0_1";
#endif
        NetProcess.SendRequest<PayItemId>(req, ProtoIdMap.CMD_SendPayInfo, (msg) =>
        {
            PayInfoBack data = msg.Read<PayInfoBack>();
            if (data.code == 1)
            {
                
                PayInfoItem pdata = data.data.payData;
                //*@param appKey 签名
                //*@param ip ip
                //*@param subject 商品名称
                //*@param amount 订单总金额，单位分
                //*@param body 商品描述
                //*@param orderNo 订单号
                //*@param notifyUrl 支付结果通知地址
                //*@param cpChannel 渠道，（非必填）
		        //*@param description 订单附加描述（非必填）
		        //*@param extra 附加数据（非必填）
		        //*@param paySytle 支付方式 1：微信  2：支付宝
                //* @param appInfo APP信息  应用名 | 包名
                SixqinSDKManager.Inst.SendMsg(SixqinSDKManager.RECHARGE, pdata.appKey, pdata.ip, v, int.Parse(pdata.amount), desc, pdata.orderNo, pdata.notifyUrl, "", "", "", _payType.ToString(), appInfo);
                
            }
            else
                GameUtils.ShowErrorTips(data.code);
        });
    }


    public void SendGetPayUrl(int id, string pay)
    {
        SendPayRoomCardReq req = new SendPayRoomCardReq();
        req.id = id;
        req.pay = pay;
        NetProcess.SendRequest<SendPayRoomCardReq>(req, ProtoIdMap.CMD_SendGetPayUrl, (msg) =>
        {
            PayUrlBack ack = msg.Read<PayUrlBack>();
            if (ack.code == 1)
            {
                if (ack.data != null)
                    Application.OpenURL(ack.data.url);
            }
            else
            {
                GameUtils.ShowErrorTips(ack.code);
            }
        });
    }


    /// <summary>
    /// 金币换房卡
    /// </summary>
    public void GoldChangeCard(int mId)
    {
        PayItemId req = new PayItemId();
        req.id = mId;
        NetProcess.SendRequest<PayItemId>(req, ProtoIdMap.CMD_SendPayInfo, (msg) =>
        {

        });
    }


    private string GetIP(ADDRESSFAM Addfam)
    {
        //Return null if ADDRESSFAM is Ipv6 but Os does not support it
        if (Addfam == ADDRESSFAM.IPv6 && !Socket.OSSupportsIPv6)
        {
            return null;
        }

        string output = "";

        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
            NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

            if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
#endif 
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    //IPv4
                    if (Addfam == ADDRESSFAM.IPv4)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                            Debug.Log("啊" + output);
                        }
                    }

                    //IPv6
                    else if (Addfam == ADDRESSFAM.IPv6)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
        }
        return output;
    }
    
    public enum ADDRESSFAM
    {
        IPv4, IPv6
    }
}
