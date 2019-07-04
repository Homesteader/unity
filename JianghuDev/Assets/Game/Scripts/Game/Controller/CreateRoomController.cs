using UnityEngine;
using System.Collections;

public class CreateRoomController : BaseController
{

    public CreateRoomView mView;

    public CreateRoomController() : base("CreateRoomView", "Windows/CreateRoomView/CreateRoomView")
    {
        SetModel<CreateRoomModel>();
    }

    public override BaseView OpenWindow()
    {
        BaseView view = base.OpenWindow();
        if (mView == null)
            mView = view as CreateRoomView;
        return view;
    }
}
