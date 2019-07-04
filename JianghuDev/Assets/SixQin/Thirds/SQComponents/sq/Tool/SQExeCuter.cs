using UnityEngine;
using System.Collections;

public class SQExeCuter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
    
    
    public void Execute()
    {

    }
    
    
    IEnumerable Exe()
    {
        yield return new WaitForSeconds(0.1f);
    }	



}
