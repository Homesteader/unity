using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            Debug.Log("down");
            StartCoroutine("Fade");
        }
    }

    IEnumerator Fade()
    {
        for (float i = 1f; i >= 0; i-= 0.1f)
        {
            Color color = GetComponent<Renderer>().material.color;
            color.a = i;
            Debug.Log(color);
            GetComponent<Renderer>().material.color = color;
            yield return new WaitForSeconds(.1f);
            
        }
    }
}
