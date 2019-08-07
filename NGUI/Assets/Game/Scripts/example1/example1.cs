using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class example1 : MonoBehaviour
{
    public UILabel mLabel;
    // Start is called before the first frame update
    void Start()
    {
        setData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void setData()
    {

        mLabel.text = "斯内德发甲方将减肥减肥的斯内德发甲方将减肥" + "[b]毛泽东[/b]" +
                      "减肥的斯内德发甲方将减肥减肥的斯内德发甲方将" + "[i]毛泽东[/i]" +
                      "减肥减\n肥的斯内德发甲方将减肥减肥的斯内德发甲方" + "[u]毛泽东[/u]" +
                      "将减肥减肥的肥的斯内德发甲方将减肥减肥的斯内德发甲方" + "[s]毛泽东[/s]" +
                      "将减肥减肥的肥的斯内德发甲方将减肥减肥的斯内德发甲方" + "[FAA000]毛泽东[-]" +
                      "将减肥减肥的肥的斯内德发甲方将减肥减肥的斯内德发甲方" + "[c]毛泽东[/c]" +
                      "将减肥减肥的肥的斯内德发甲方将减肥减肥的斯内德发甲方" + "[sup]毛泽东[/sup]" +
                      "将减肥减肥的肥的斯内德发甲方将减肥减肥的斯内德发甲方" + "[sub]毛泽东[/sub]" +
                      "将减肥减肥的肥的斯内德发甲方将减肥减肥的斯内德发甲方" + "[url=http://www.baidu.com]URL毛泽东[/url]哈哈哈";
    }
    
    void OnClick ()
    {
        string url = mLabel.GetUrlAtPosition(UICamera.lastWorldPosition);
        if (!string.IsNullOrEmpty(url)) Application.OpenURL(url);
    }
}
