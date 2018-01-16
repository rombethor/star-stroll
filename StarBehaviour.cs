using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StarBehaviour : MonoBehaviour {

	public Text t_toCopy;

	public Star star;
	/*
	public string starName;
	public int nameLevel;

	public string hip;

	public string spectralType;
	public float magnitude;
	public float luminosity;
	public string constelation;*/

	//public Vector3 pos;

	//public float ra, dist;

	private Text guitext;

	public CameraBehaviour.Projection inProjection;  //the projection coordinates in which this star is presently sitting

	public char type;

	private Renderer rend;

	public bool isShowingPlanets = false;

	//public string planetText;
	//public int planetCount;
	public bool planetTextLoaded;

	private MeshCollider mc;

	// Use this for initialization
	void Start () {
		//t = GetComponentsInChildren<Text>()[0];
		rend = GetComponent<Renderer>();
		planetTextLoaded = false;

		inProjection = CameraBehaviour.Projection.RA_DIST;

		Vector3 temp = new Vector3();
		temp.x = Mathf.Cos(star.ra) * star.dist;
		temp.z = Mathf.Sin (star.ra) * star.dist;
		temp.y = 0;
		transform.position = temp;
		
		transform.forward = new Vector3(0, 0, 1);
	}

	// Update is called once per frame
	void LateUpdate () {
		if( inProjection != CameraBehaviour.projection )
		{
			if( CameraBehaviour.projection == CameraBehaviour.Projection.PERSPECTIVE )
			{
				transform.position = star.pos;
			}
			else if( CameraBehaviour.projection == CameraBehaviour.Projection.FLAT_3D )
			{
				transform.position = new Vector3(star.pos.x, 0f, star.pos.z);

				transform.forward = new Vector3(0f, 0f, 1f);
			}
			else //RA_Dist
			{
				/*
				Vector3 temp = new Vector3();
				temp.x = Mathf.Cos(star.ra) * star.dist;
				temp.z = Mathf.Sin (star.ra) * star.dist;
				temp.y = 0;
				transform.position = temp;*/

				transform.position = star.ra_dist;

				transform.forward = new Vector3(0, 0, 1);

			}
			inProjection = CameraBehaviour.projection;
		}

		if( Camera.main != null && LoadStars.loadingFinished == true )
		{

			if( rend.isVisible == true )
			{
                
				float distance = (transform.position - Camera.main.transform.position).magnitude;
                //ADD OR REMOVE MESH COLLIDER FOR STAR SELECTION
                if ( mc == null )
				{
					if( distance <= 350f )
						mc = gameObject.AddComponent<MeshCollider>();
				}
				else if( mc != null )
				{
					if( distance > 350f )
						Destroy(mc);
				}
				///Face Camera
				if( Camera.main.orthographic == false )
				{
					transform.LookAt(Camera.main.transform.position);
					transform.Rotate(90.0f, 0.0f, 0.0f);
				}
				//if( Camera.main.orthographic == true || (transform.position - Camera.main.transform.position).magnitude <= 35 )
				//{
				if( (Camera.main.orthographic && Camera.main.orthographicSize < star.nameLevel * 2 )  || star.nameLevel >= 6 )
				{
					if( Camera.main.orthographic || distance <= 350 )
					{
						if( guitext == null && LoadStars.textLevel <= star.nameLevel )
						{
							guitext = Instantiate( t_toCopy ) as Text;

							guitext.rectTransform.SetParent(LoadStars.canvas.transform, true);

							guitext.text = star.name;
						}
						else if( LoadStars.textLevel > star.nameLevel )
						{
							Destroy(guitext);
							guitext = null;
						}
						else if( guitext != null && LoadStars.textLevel <= star.nameLevel )
						{
							Vector3 temp = Camera.main.WorldToViewportPoint(transform.position);
							temp.y *= Screen.height;
							temp.x *= Screen.width;
							guitext.rectTransform.position = temp;
						}
					}
					else if( guitext != null )
					{
						Destroy(guitext);
						guitext = null;
					}
				}
				else if( guitext != null )
				{
					Destroy(guitext);
					guitext = null;
				}
				//}
			}
			else
			{
				if( guitext != null )
				{
					Destroy(guitext);
					guitext = null;
				}

				if( mc != null )
				{
					Destroy(mc);
					mc = null;
				}
			}


		}
	}

	void OnDestroy()
	{
		if( guitext != null )
		{
			Destroy(guitext);
		}
	}

}
