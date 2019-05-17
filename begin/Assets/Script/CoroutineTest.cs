using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        if (Input.GetKeyDown("p"))
        {
            StartCoroutine(ScreenShotPNG());
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

    IEnumerator ScreenShotPNG()
    {
        yield return new WaitForEndOfFrame();
        
        int w = Screen.width;
        int h = Screen.height;
        
        Texture2D texture2D = new Texture2D(w,h,TextureFormat.RGB24,false);
        texture2D.ReadPixels(new Rect(0,0,w,h),0,0 );
        texture2D.Apply();
        byte[] bytes = texture2D.EncodeToPNG();
        Destroy(texture2D);

        File.WriteAllBytes(Application.dataPath + "/../screen.png",bytes);
    }
}
