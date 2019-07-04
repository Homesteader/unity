using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class SQTimeOutTool : MonoBehaviour {

	public static SQTimeOutTool It;
	//
	private Queue mDelayQue = new Queue(); 

	
	public delegate void SQTimeOutDeleteCall();
	public List<SQTimeOutDeleteCall> mDelayList;




	void Awake(){
		It = this;
	}

	// Use this for initialization
	void Start () {
	
	}

	public void AddDelay(SQTimeOutDeleteCall call, float timeOut){
		StartCoroutine (TimeDown (call, timeOut));
	}


	IEnumerator TimeDown(SQTimeOutDeleteCall call, float time){
		yield return new WaitForSeconds (time);
		call ();
	}

	void Update(){

	}




}
