using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {

	public Slider pcLimitSlider;

	public Text pcLimitText;

	void Awake() {
		
	}

	// Use this for initialization
	void Start () {
		CameraBehaviour.projection = CameraBehaviour.Projection.RA_DIST;

		pcLimitSlider.value = LoadStars.parsecLimit;
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKey(KeyCode.Escape) )
		{
			Application.Quit();
		}
	}

	public void SliderOnChange()
	{
		pcLimitText.text = pcLimitSlider.value + "pc";
		LoadStars.parsecLimit = pcLimitSlider.value;
	}

	public void ButtonLoad()
	{
        UnityEngine.SceneManagement.SceneManager.LoadScene("stars");
//		Application.LoadLevel("stars");
	}
}
