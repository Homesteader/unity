using UnityEngine;
using System.Collections;

public class WindowsRoot : MonoBehaviour {

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
