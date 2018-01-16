using UnityEngine;
using System.Collections;

public class LoadingBarHide : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if( LoadStars.loadingFinished )
			this.gameObject.SetActive(false);
	}
}
