using UnityEngine;
using System.Collections;

public class WifiComponent : MonoBehaviour {


    public GameObject[] Icons;

    // Use this for initialization
    void Start () {
	
	}

    /// <summary>
    /// 1~3
    /// </summary>
    public void SetStrength(int strength)
    {
        if(strength >= Icons.Length)
        {
            return;
        }

        for(int i = 0; i < Icons.Length; i++)
        {
            Icons[i].SetActive(false);
        }

        Icons[strength].SetActive(true);

    }



}
