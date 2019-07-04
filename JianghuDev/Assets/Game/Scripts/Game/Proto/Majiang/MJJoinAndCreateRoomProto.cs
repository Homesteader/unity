using UnityEngine;
using System.Collections;

public class MJCreateRoomProto: MsgResponseBase
{
    public MJCreateRoomData data;
}

public class MJCreateRoomData
{
    public string host;
    public string port;
    public int roomId;
}
