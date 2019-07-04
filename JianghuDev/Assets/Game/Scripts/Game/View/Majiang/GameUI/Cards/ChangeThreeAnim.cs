using UnityEngine;
using System.Collections;

public class ChangeThreeAnim : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    public float augle = 0;
    // Update is called once per frame
    void Update()
    {
        if (augle < 0)
        {
            transform.Rotate(new Vector3(0, -1, 0), -augle * Time.deltaTime);
            if (transform.localRotation.eulerAngles.y <=360+augle+3 )
            {
                transform.localRotation = Quaternion.Euler(new Vector3(0, 360 + augle, 0));
            }
        }
        else if (augle > 0)
        {
            transform.Rotate(new Vector3(0, 1, 0), augle * Time.deltaTime);
            if (transform.localRotation.eulerAngles.y >= augle - 3)
            {
                transform.localRotation = Quaternion.Euler(new Vector3(0, augle, 0));
            }
        }
       
    }
}
