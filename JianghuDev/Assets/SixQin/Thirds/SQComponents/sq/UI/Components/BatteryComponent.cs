using UnityEngine;
using System.Collections;

public class BatteryComponent : MonoBehaviour {

    public GameObject[] Icons;

    // Use this for initialization
    void Start()
    {

    }

    /// <summary>
    /// 1~5
    /// </summary>
    public void SetStrength(int strength)
    {
        if (strength >= Icons.Length)
        {
            return;
        }

        for (int i = 0; i < Icons.Length; i++)
        {
           if( i < strength)
            {
                Icons[i].SetActive(true);
            }else
            {
                Icons[i].SetActive(false);
            }
        }

    }

}
