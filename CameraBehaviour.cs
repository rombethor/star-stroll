using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {

	public float perspectiveZoomSpeed = 0.15f;
	public float orthographicZoomSpeed = 0.1f;

	public float rotateSpeed = 25.0f;

	public float translateSpeed = 0.3f;

	public GameObject target;

	public static bool guiTouch = false;  //true when mouse or touch is on gui.

	private int touchId;

	private float touchTime;
	private Vector2 touchVec;  //This is also used for the mouse control

	//bool touch_cancelled = false;

	public enum Projection {RA_DIST, PERSPECTIVE, FLAT_3D};

	public static Projection projection = Projection.RA_DIST;

	public Camera cam;

	public CanvasController canvas;


	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
		rotateSpeed = 90.0f/Screen.width;
        cam.layerCullSpherical = true;
        cam.farClipPlane = LoadStars.parsecLimit;
	}
	
	// Update is called once per frame
	void Update () {

		if( cam == null )
		{
			cam = GetComponent<Camera>();
			if( cam == null )
			{
				Debug.Log ("No Camera Found!");
				Application.Quit();
			}
		}

		if( Input.GetMouseButton(0) )
		{
            if (!canvas.IsOverGUI(new Vector2(Input.mousePosition.x, Input.mousePosition.y)))
            {
                guiTouch = false;

                Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.SphereCast(ray, 0.35f, out hit))
                {
                    changeTarget(hit.collider.gameObject);
                }
            }
        }

        if( Input.GetMouseButton(1) )
        {
            if (!canvas.IsOverGUI(touchVec))
            {
                Vector2 change = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - touchVec;
                //touchVec = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                if (!cam.orthographic)
                {
                    Vector3 diff = target.transform.position - cam.transform.position;
                    //Maintain this ordering of rotations!
                    cam.transform.RotateAround(target.transform.position, new Vector3(diff.z, 0f, -diff.x), -rotateSpeed * change.y);
                    cam.transform.RotateAround(target.transform.position, new Vector3(0f, 1f, 0f), rotateSpeed * change.x);
                    ///NEED:  TO limit rotation around diff so that the camera doesn't flip.
                }
                else
                {
                    touchVec = new Vector2(Input.mousePosition.x, Input.mousePosition.y); //to avoid jumping camera
                    float parsecPerPixel = cam.orthographicSize / Screen.width;
                    //Vector2 delta = touch.deltaPosition * (Time.deltaTime / touch.deltaTime);
                    cam.transform.Translate(new Vector3(-translateSpeed * change.x * parsecPerPixel/2, -translateSpeed * change.y * parsecPerPixel/2, 0f));
                }
            }
        }
        else
        {
            touchVec = new Vector2(Input.mousePosition.x, Input.mousePosition.y); //to avoid jumping camera
        }

        if( Input.GetAxis("Mouse ScrollWheel") != 0 )
        {
            float change = Input.GetAxis("Mouse ScrollWheel") * 3;
            if (cam.orthographic)
            {
                cam.orthographicSize += -change * orthographicZoomSpeed;

                cam.orthographicSize = Mathf.Max(GetComponent<Camera>().orthographicSize, 0.1f);
            }
            else
            {
                if (change < 0)
                {
                    float difference = (cam.transform.position - target.transform.position).magnitude;
                    if (difference - change * perspectiveZoomSpeed >= 0.5)
                    {
                        cam.transform.Translate(new Vector3(0f, 0f, change * perspectiveZoomSpeed));
                    }
                }
                else
                {
                    cam.transform.Translate(new Vector3(0f, 0f, change * perspectiveZoomSpeed));
                }

            }
        }

		if( Input.GetKey(KeyCode.Escape) )
		{
            UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
//			Application.LoadLevel("menu");  DEPRICATED
		}

		if( Input.multiTouchEnabled )
		{
			if( Input.touchCount == 1 )
			{
				Touch touch = Input.GetTouch(0);

				TouchPhase phase = touch.phase;

				//Check UI (layer 5) first to check
				//touch_cancelled = canvas.IsOverGUI(touchVec);


					switch( phase )
					{
					case TouchPhase.Began:
					{
						touchVec = touch.position;
						touchTime = Time.time;
					}
						break;
					case TouchPhase.Moved:
					{
						if( !canvas.IsOverGUI(touchVec) )
						{
							Vector2 change = touch.position - touchVec;
							touchVec = touch.position;
							if( ! cam.orthographic )
							{
								Vector3 diff = target.transform.position - cam.transform.position;
								cam.transform.RotateAround(target.transform.position, new Vector3(0f,1f, 0f), rotateSpeed*change.x );
								cam.transform.RotateAround(target.transform.position, new Vector3(diff.z,0f, -diff.x), -rotateSpeed*change.y);
							}
							else
							{
								float parsecPerPixel = cam.orthographicSize/Screen.width;
								//Vector2 delta = touch.deltaPosition * (Time.deltaTime / touch.deltaTime);
								cam.transform.Translate(new Vector3(- translateSpeed * change.x * parsecPerPixel, - translateSpeed*change.y * parsecPerPixel, 0f));
							}
						}
					}
						break;
					case TouchPhase.Ended:
					{
						if( !canvas.IsOverGUI(touchVec)  && !canvas.IsOverGUI(touch.position) )
						{
							if( (touchVec - touch.position).magnitude < 5.0f )
							{
								if( Time.time - touchTime < 0.20f )
								{
									Ray ray = GetComponent<Camera>().ScreenPointToRay(touch.position);
									
									RaycastHit hit;
									if( Physics.SphereCast(ray, 0.25f, out hit) )
									{
										changeTarget(hit.collider.gameObject);
									}
								}
							}
						}
					}
						break;
					} //end switch
				
			}
			if( Input.touchCount == 2 )
			{
				Touch touch1 = Input.GetTouch(0);
				Touch touch2 = Input.GetTouch(1);

				Vector2 touch1posprev = touch1.position - touch1.deltaPosition;
				Vector2 touch2posprev = touch2.position - touch2.deltaPosition;

				float distPrev = (touch1posprev - touch2posprev).magnitude;
				float dist = (touch1.position - touch2.position).magnitude;

				float change = dist - distPrev;

				//change camera zoom
				if(cam.orthographic )
				{
					cam.orthographicSize += -change * orthographicZoomSpeed;

					cam.orthographicSize = Mathf.Max(GetComponent<Camera>().orthographicSize, 0.1f);
				}
				else
				{
					if( change < 0 )
					{
						float difference = (cam.transform.position - target.transform.position).magnitude;
						if( difference - change*perspectiveZoomSpeed >= 0.5)
						{
							cam.transform.Translate(new Vector3(0f, 0f, change*perspectiveZoomSpeed));
						}
					}
					else
					{	
						cam.transform.Translate(new Vector3(0f, 0f, change*perspectiveZoomSpeed));
					}

				}

			}
		}//end multitouch check


#if UNITY_EDITOR
		if( Input.GetKey(KeyCode.A) )
		{
			if( cam.orthographic )
				cam.transform.Translate(-0.4f * Time.deltaTime, 0f, 0f);
			else
				cam.transform.RotateAround(target.transform.position, new Vector3(0f,1f, 0f), 50f*rotateSpeed*Time.deltaTime );
		}
		if( Input.GetKey(KeyCode.D) )
		{
			if( cam.orthographic )
				cam.transform.Translate(0.4f * Time.deltaTime, 0f, 0f);
			else
				cam.transform.RotateAround(target.transform.position, new Vector3(0f, 1f, 0f), -50f*rotateSpeed * Time.deltaTime);
		}
		if( Input.GetKey(KeyCode.W) )
		{
			if( cam.orthographic )
			{
				cam.transform.Translate(0f, 0.4f * Time.deltaTime, 0f);
			}
			else
			{
				Vector3 diff = target.transform.position - cam.transform.position;
				cam.transform.RotateAround(target.transform.position, new Vector3(diff.z,0f, -diff.x), 50f*rotateSpeed*Time.deltaTime);
			}
		}
		if( Input.GetKey(KeyCode.S) )
		{
			if( cam.orthographic )
			{
				cam.transform.Translate(0f, -0.4f * Time.deltaTime, 0f);
			}
			else
			{
				Vector3 diff = target.transform.position - cam.transform.position;
				cam.transform.RotateAround(target.transform.position, new Vector3(diff.z,0f, -diff.x), -50f*rotateSpeed*Time.deltaTime);
			}
		}

		if( cam.orthographic )
		{
			if( Input.GetKey(KeyCode.Z) )
			{
				cam.orthographicSize -= 5*orthographicZoomSpeed * Time.deltaTime;
			}
			else if( Input.GetKey(KeyCode.X) )
			{
				cam.orthographicSize += 5*orthographicZoomSpeed * Time.deltaTime;
			}
		}

#endif


		/*

		if( cam.orthographic == false )
		{
			if( Input.GetKey(KeyCode.A) )
			{
				transform.RotateAround(target.transform.position, new Vector3(0f, 1f, 0f), rotateSpeed*Time.deltaTime);
			}
			else if( Input.GetKey(KeyCode.D) )
			{
				transform.RotateAround(target.transform.position, new Vector3(0f, 1f, 0f), -rotateSpeed*Time.deltaTime);
			}
		}
		*/

		//Update camera facing

	}

	public void ClickRaycastToTarget()
	{
			Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;
			Debug.Log( canvas.IsOverGUI(Input.mousePosition) );
			if( Physics.SphereCast(ray, 0.25f, out hit) )
			{
				changeTarget(hit.collider.gameObject);
			}
	}

	void changeTarget(GameObject nTarget)
	{
		target = nTarget;

		if( cam.orthographic )
		{
			transform.rotation = Quaternion.identity;
			transform.forward = new Vector3(0.0f, -1.0f, 0.0f);
			transform.position = new Vector3(target.transform.position.x, 10.0f, target.transform.position.z);
		}
		else //perspective camera
		{
			StartCoroutine(zoomToTarget());
		}
	}


	IEnumerator slerpToTarget()
	{
		float slerpToTargetStartTime = Time.time;

		Quaternion lookRot = Quaternion.LookRotation(target.transform.position - transform.position, new Vector3(0f, 0f, 1f));
		while( (Time.time - slerpToTargetStartTime) < 2.0f )
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRot,  (Time.time - slerpToTargetStartTime) / 2f);
			yield return null;
		}
		yield return null;
	}

	IEnumerator zoomToTarget()
	{
		StarBehaviour sb = target.GetComponent<StarBehaviour>();
		Star s = sb.star;

		float zoomStartTime = Time.time;
		Vector3 startPos = transform.position;

		///on the same plane as the star:
		Vector3 flatPos = new Vector3(startPos.x, s.posY, startPos.z);

		//rethink this
		if( flatPos.magnitude == 0 )  //In case we are directly above the target
		{
			flatPos.x = -2.0f;
		}

		Vector3 destination = (flatPos - s.pos).normalized * 2.5f;  //The difference
		destination = s.pos + destination; //The difference between the target and cam
		destination.y = s.posY; //relocate the same y plane

		Quaternion lookRot = Quaternion.identity;
		Vector3 up = new Vector3(0f, 1f, 0f);
		while( (Time.time - zoomStartTime) < 2.1f )
		{
			if( projection != Projection.PERSPECTIVE ) break;
			transform.position = Vector3.Lerp(startPos, destination, (Time.time - zoomStartTime)/2f);

			lookRot = Quaternion.LookRotation((target.transform.position - transform.position), up);

			transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, (Time.time-zoomStartTime)/2f);

			yield return null;
		}
		yield return null;
	}

	public void makeOrthographic()
	{
		cam.orthographic = true;

		transform.forward = new Vector3(0.0f, -1.0f, 0.0f);

		transform.position = new Vector3(target.transform.position.x, 10f, target.transform.position.z);
	}

	public void followTargetOnProjectionChange()
	{
		StarBehaviour sb = target.GetComponent<StarBehaviour>();
		if( sb != null )
		{
			Vector3 temp = sb.star.pos;
			if( projection ==  Projection.RA_DIST )
			{
				temp = sb.star.ra_dist;
				temp.y += 2f;
			}
			else if( projection == Projection.FLAT_3D )
			{
				temp.y = 2f;
			}
			else
			{
				temp.y = 0;
				temp.x += 2f;
				Debug.Log(sb.star.pos);
				Debug.Log(temp);
			}
			transform.position = temp;
			transform.forward = new Vector3(0f, -1f, 0f);
		
		}
		else
			Debug.Log("Star Behaviour not Found: followTargetOnProjectionChange");
	}

	public void changeProjection(Projection proj)
	{
		if( proj != projection )
		{
			switch(proj)
			{
			case Projection.FLAT_3D:
			{
				projection = proj;
				makeOrthographic();
				followTargetOnProjectionChange();
			}
				break;
			case Projection.PERSPECTIVE:
			{
				projection = proj;
				LoadStars.textLevel = 5;
				cam.orthographic = false;
				followTargetOnProjectionChange();
				StartCoroutine(zoomToTarget());
			}
				break;
			case Projection.RA_DIST:
			{
				projection = proj;

				makeOrthographic();
				followTargetOnProjectionChange();
			}
				break;
			}
		}
	}

	public void toggleProjection()
	{
		if( projection == Projection.FLAT_3D )
		{
			changeProjection(Projection.RA_DIST);
		}
		else if(projection == Projection.RA_DIST)
		{
			changeProjection(Projection.PERSPECTIVE);
		}
		else
		{
			changeProjection(Projection.FLAT_3D);
		}
	}
}
