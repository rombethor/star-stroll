using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasController : MonoBehaviour {

	public GameObject infoWindow;
    public StarGraphicLoader graphicLoader;
	public RectTransform buttonMenu;
	public RectTransform buttonProjection;
	public RectTransform buttonInfo;
	public RectTransform buttonHome;
	public RectTransform buttonColour;

	public Text projectionButtonText;

	public Text colourButtonText;
    public Text sizeButtonText;

	public Camera cam;

	public CameraBehaviour camBeh;

	public Canvas canvas;


	private Rect rectInfo;
	private Rect rectHome;
	private Rect rectMenu;
	private Rect rectProjection;
	private Rect rectColour;
	private Rect rectWindow;


	// Use this for initialization
	void Start () {
		camBeh = cam.GetComponent<CameraBehaviour>();
		canvas = GetComponent<Canvas>();

		LayoutRects();
	}
	
	// Update is called once per frame
	void Update () {
		
		//Debug.Log ("Info: " + buttonInfo.rect);
		//Debug.Log ("Home: " + buttonHome.rect);
		//Debug.Log ("Home: " + buttonInfo.position);
	}

	public void LayoutRects()
	{
		rectInfo = buttonInfo.rect;
		rectInfo.x += buttonInfo.position.x;
		rectInfo.y += buttonInfo.position.y;

		Debug.Log("rectInfo: " + rectInfo);

		rectHome = buttonHome.rect;
		rectHome.x += buttonHome.position.x;
		rectHome.y += buttonHome.position.y;

		rectProjection = buttonProjection.rect;
		rectProjection.x += buttonProjection.position.x;
		rectProjection.y += buttonProjection.position.y;

		rectMenu = buttonMenu.rect;
		rectMenu.x += buttonMenu.position.x;
		rectMenu.y += buttonMenu.position.y;

		rectColour = buttonColour.rect;
		rectColour.x += buttonColour.position.x;
		rectColour.y += buttonColour.position.y;

		RectTransform infoRectTrans = infoWindow.GetComponent<RectTransform>();
		rectWindow = infoRectTrans.rect;
		rectWindow.x += infoRectTrans.position.x;
		rectWindow.y += infoRectTrans.position.y;
	}
	
	public bool IsOverGUI(Vector2 pos)
	{
		if( rectColour.Contains(pos) )
			return true;
		if( infoWindow.activeSelf )
		{
			if( rectWindow.Contains(pos) )
				return true;
		}
		if( rectProjection.Contains(pos) )
			return true;
		if( rectMenu.Contains(pos) )
			return true;
		if( rectInfo.Contains(pos) )
			return true;
		if( rectHome.Contains(pos) )
			return true;

		return false;
	}

	public void ButtonColour()
	{
		StarGraphicLoader.planetaryHighlight = !StarGraphicLoader.planetaryHighlight;

		if( ! StarGraphicLoader.planetaryHighlight )
			colourButtonText.text = "Colour";
		else
			colourButtonText.text = "Planets";

	}

    public void ButtonSize()
    {
        StarGraphicLoader.sizeByLuminosity = !StarGraphicLoader.sizeByLuminosity;

        if (!StarGraphicLoader.sizeByLuminosity)
            sizeButtonText.text = "Size: Point";
        else
            sizeButtonText.text = "Size: Lux";
        
    }

	public void ButtonInfo()
	{
		if( infoWindow != null )
		{
			infoWindow.SetActive(true);
		}
	}

	public void ButtonProjection()
	{
		//called in addition to CameraBehaviour.toggleProjection;
		if( CameraBehaviour.projection == CameraBehaviour.Projection.FLAT_3D )
		{
			projectionButtonText.text = "3D_FLAT";
		}
		else if( CameraBehaviour.projection == CameraBehaviour.Projection.PERSPECTIVE )
		{
			projectionButtonText.text = "3D";
		}
		else if( CameraBehaviour.projection == CameraBehaviour.Projection.RA_DIST )
		{
			projectionButtonText.text = "RA_DIST";
		}
	}

	public void ButtonHome()
	{
		//Return to the sun
		cam.transform.position = new Vector3( 0f, 5f, 0f );
		cam.transform.forward = new Vector3( 0f, -1f, 0f );

	}


	public void ButtonMenu()
	{
        UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
//		Application.LoadLevel("menu");
	}
}
