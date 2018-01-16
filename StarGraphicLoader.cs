using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StarGraphicLoader : MonoBehaviour {

	GameObject[] gameObjects;

	public Text textPrefab;

	Mesh m;

	public Material mB;
	public Material mA;
	public Material mF;
	public Material mG;
	public Material mK;
	public Material mM;

	public Material mPlanetary;
	public static bool planetaryHighlight = false;
    public static bool sizeByLuminosity = false;

	public Transform camTransform;

	public CameraBehaviour camB;

	// Use this for initialization
	void Start () {

		m = new Mesh();
		m.vertices = new Vector3[]{
			new Vector3(-0.05f, 0f, 0.05f),
			new Vector3(0.05f, 0f, 0.05f),
			new Vector3(0.05f, 0f, -0.05f),
			new Vector3(-0.05f, 0f, -0.05f)
		};
		m.uv = new Vector2[]{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		};
		m.triangles = new int[]{0, 1, 2, 0, 2, 3};
		m.RecalculateNormals();

		StartCoroutine(CheckStars ());
	}
	
	// Update is called once per frame
	void Update () {
	}
	

	IEnumerator CheckStars()
	{
		while( !LoadStars.loadingFinished )
		{yield return null;}
		gameObjects = new GameObject[LoadStars.starsLength];
		float sqrParsecLimit = LoadStars.parsecLimit*LoadStars.parsecLimit;
        //Refresh visible stars
		while( Application.isPlaying )
		{
			yield return null; //pause one frame
			int lb = 0;
			for( int i = 0; i < LoadStars.starsLength; i++ )
			{
				if( LoadStars.stars[i] == null )
				{
					Debug.Log("Fudge!");
				}
				else
				{
                    //Check VISIBLE DISTANCES
					bool show = false;
					if( CameraBehaviour.projection == CameraBehaviour.Projection.PERSPECTIVE )
					{
						if( (LoadStars.stars[i].pos - camB.target.transform.position).sqrMagnitude <= sqrParsecLimit)
						{
							show = true;
						}
					}
					else if( CameraBehaviour.projection == CameraBehaviour.Projection.FLAT_3D )
					{
						Vector3 temp = LoadStars.stars[i].pos - camTransform.position;
						temp.y = 0;
						if( temp.sqrMagnitude <= sqrParsecLimit )
						{
							show = true;
						}
					}
					else if( CameraBehaviour.projection == CameraBehaviour.Projection.RA_DIST )
					{
						if( (LoadStars.stars[i].ra_dist - camTransform.position).sqrMagnitude <= sqrParsecLimit)
						{
							show = true;
						}
					}
					if( show == true )
					{
						if( gameObjects[i] == null && LoadStars.stars[i] != null )
						{
							gameObjects[i] = new GameObject(LoadStars.stars[i].name);

							MeshRenderer mr = gameObjects[i].AddComponent<MeshRenderer>();
							MeshFilter mf = gameObjects[i].AddComponent<MeshFilter>();

							mf.mesh = m;
                            

							if( LoadStars.stars[i].spectralType.Length > 0 )
							{
								char type = LoadStars.stars[i].spectralType[0];
								
								if( type == 'G' )
									mr.material = mG; // = new Color(225, 159, 100);
								else if(type == 'A' || type == 'a' )
									mr.material = mA;
								else if(type == 'B' || type == 'b' )
									mr.material = mB;
								else if(type == 'F' || type == 'f' )
									mr.material = mF;
								else if(type == 'K' || type == 'k' )
									mr.material = mK;
								else if(type == 'M' || type == 'm')
									mr.material = mM;
								else
									mr.material = mG;
							}
							else
							{
								mr.material = mG;
							}

                            StarBehaviour sb = gameObjects[i].AddComponent<StarBehaviour>();

							gameObjects[i].transform.position = LoadStars.stars[i].pos;
                            //Edited:  Moved size by luminosity to after new gameobject check


							sb.star = LoadStars.stars[i];

							sb.t_toCopy = textPrefab;

						}
						else //if gameobject is ALREADY DEFINED
						{
							if( LoadStars.stars[i].has_planets )
							{
								StarBehaviour sb = gameObjects[i].GetComponent<StarBehaviour>();
								if( sb != null )
								{
									if( planetaryHighlight )
									{
										if( ! sb.isShowingPlanets )
										{
											gameObjects[i].GetComponent<MeshRenderer>().material = mPlanetary;
											sb.isShowingPlanets = true;
										}
									}
									else
									{
										if( sb.isShowingPlanets )
										{

											if( LoadStars.stars[i].spectralType.Length > 0 )
											{
												char type = LoadStars.stars[i].spectralType[0];
												
												if( type == 'G' )
													gameObjects[i].GetComponent<MeshRenderer>().material = mG; // = new Color(225, 159, 100);
												else if(type == 'A' || type == 'a' )
													gameObjects[i].GetComponent<MeshRenderer>().material = mA;
												else if(type == 'B' || type == 'b' )
													gameObjects[i].GetComponent<MeshRenderer>().material = mB;
												else if(type == 'F' || type == 'f' )
													gameObjects[i].GetComponent<MeshRenderer>().material = mF;
												else if(type == 'K' || type == 'k' )
													gameObjects[i].GetComponent<MeshRenderer>().material = mK;
												else if(type == 'M' || type == 'm')
													gameObjects[i].GetComponent<MeshRenderer>().material = mM;
												else
													gameObjects[i].GetComponent<MeshRenderer>().material = mG;
											}
											else
											{
												gameObjects[i].GetComponent<MeshRenderer>().material = mG;
											}
											sb.isShowingPlanets = false;
										}
									} //end of planetary highlight check

                                    //Size by luminosity?
								}
								else
									Debug.Log("Starbehaviour was null!");
							}
							//if has_has planets, check that its material is correct
							//

						}
                        if (sizeByLuminosity)
                        {
                            float scaler = 1;
                            float.TryParse(LoadStars.stars[i].luminosity, out scaler);
                            scaler = Mathf.Sqrt(scaler);
                            gameObjects[i].transform.localScale = new Vector3(scaler, scaler, scaler);
                        }
                        else
                        {
                            gameObjects[i].transform.localScale = new Vector3(1, 1, 1);
                        }
                        lb++;
						if( lb >= 3000 ){ lb = 0; yield return null; }
					}
					else
					{
						if( gameObjects[i] != null )
						{
							if( camB.target != gameObjects[i] )
								Destroy(gameObjects[i]);
						}
					}
				}
			}
		yield return null;
		}
		yield return null;
	}

    public void RefreshStars()
    {
        ClearStars();
        
    }

	public void ClearStars()
	{
		for( int i = 0; i < gameObjects.Length; i++)
			if( gameObjects[i] != null ) Destroy(gameObjects[i]);
	}
}
