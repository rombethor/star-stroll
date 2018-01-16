using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using UnityEngine.Advertisements;

//using System;
//using System.Data;
//using System.Data.Odbc;
//using System.Runtime.InteropServices;

#if UNITY_EDITOR
using UnityEditor;
#endif
//using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class Star
{

	public char[] nameC = new char[20];
	public string name
	{
		get{ return new string(nameC); }
		set{ if( value.Length <= 20 ) nameC = value.PadRight(20).ToCharArray(); }
	}

	public char[] hippC = new char[6];
	public string hipp{
		get{ return new string(hippC); }
		set{ if( value.Length <= 6 ) hippC = value.PadRight(6).ToCharArray(); }
	}

	public Vector3 pos
	{
		get{ //SWITCH Y AND Z TO REFLECT EARTH OBSERVATION
			return new Vector3(posX, posZ, posY);
		}
	}

	public float posX;
	public float posY;
	public float posZ;

	public float ra = 0f;
	public float dist = 100000f;

	public Vector3 ra_dist;

	public int nameLevel = 0; //To show less names with more stars


	public char[] spectralTypeC = new char[10];
	public string spectralType
	{
		get{ return new string(spectralTypeC); }
		set{ if( value.Length <= 10 ) spectralTypeC = value.PadRight(10).ToCharArray(); }
	}

	public char[] luminosityC = new char[13];
	public string luminosity{
		get{ return new string(luminosityC); }
		set{ if( value.Length <= 13 ) luminosityC = value.PadRight(13).ToCharArray(); }
	}
	
	public char[] constelationC = new char[8];
	public string constelation
	{
		get{ return new string(constelationC); }
		set{ if( value.Length <= 8 ) constelationC = value.PadRight(8).ToCharArray(); }
	}

	public bool has_planets = false;

	public string planetText = "";
	public int planetCount = 0;
	public bool planetTextLoaded = false;


	public void write(BinaryWriter bw)
	{
		bw.Write(nameC,0,20);
		bw.Write(hippC,0,6);
		bw.Write(posX);
		bw.Write(posY);
		bw.Write(posZ);
		bw.Write(ra);
		bw.Write(dist);
		bw.Write(nameLevel);
		bw.Write(spectralTypeC,0,10);
		bw.Write(luminosityC,0,13);
		bw.Write(constelationC,0,8);
		bw.Write(has_planets);
	}

	public void read(BinaryReader br)
	{

		nameC = br.ReadChars(20);
		hippC = br.ReadChars(6);
		posX = br.ReadSingle();
		posY = br.ReadSingle();
		posZ = br.ReadSingle();
		ra = br.ReadSingle();
		dist = br.ReadSingle();
		nameLevel = br.ReadInt32();
		spectralTypeC = br.ReadChars(10);
		luminosityC = br.ReadChars(13);
		constelationC = br.ReadChars(8);

		has_planets = br.ReadBoolean();

		ra_dist = new Vector3(Mathf.Cos(ra) * dist, 0, Mathf.Sin(ra) * dist);
	}
};

public class LoadStars : MonoBehaviour {

	public static LoadStars starLoader;

	public static Star[] stars;

	public static int starsLength;

	public static int textLevel = 6;

	//public static int projection = 0;  //0 for RA_Dist, 1 for 3D, 2 for 3D squashed

	//the lower, the more text
	public static int starTextLevel = 6;

	public static Canvas canvas;

	public Slider loadingBar;

	public static float parsecLimit = 25.0f;

	public Camera cam;
	public CameraBehaviour camBehaviour;

	public static bool loadingFinished;

	public Material mB;
	public Material mA;
	public Material mF;
	public Material mG;
	public Material mK;
	public Material mM;

	public GameObject target;

	public Text t_toCopy;

	//private Mesh m;

	public TextAsset starData;


	// Use this for initialization
	void Awake () {
		//if( starData == null )
		//	starData = Resources.Load<TextAsset>("stars");

		DontDestroyOnLoad(this);

		if( starLoader == null )
		{
			starLoader = this;

			
			//m = GenerateStar();
			
			Debug.Log ("Starting Load..");
			//StartCoroutine( readStars(Application.persistentDataPath + "/stars.csv") );
			StartCoroutine( LoadStarsFromBinary() );
			//StartCoroutine( drawStars() );
			canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
			
			target = gameObject;



		}
		else
		{
			Destroy(this.gameObject);
		}
        
	}

	void Start()
	{
		//StartCoroutine(ShowAdvert());
	}

	// Update is called once per frame
	void Update () {

		if( canvas == null )
			canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

	}

	Mesh GenerateStar()
	{
		Mesh m = new Mesh();
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


		return m;
	}


#if UNITY_EDITOR
	[MenuItem("Tools/Format Star File")]
	private static void FormatStarsFile()
	{
		Debug.Log("Starting Binary Formatter...");

		//BinaryFormatter bf = new BinaryFormatter();
		FileStream fs = File.Open("Assets/Resources/starDB2.bytes", FileMode.OpenOrCreate);

		BinaryWriter bw = new BinaryWriter(fs);

		//Form list of stars with planets
		//
		ArrayList stars_with_planets_list = new ArrayList();
		if( true )
		{
			string[] data_1 = Resources.Load<TextAsset>("exoplanets").text.Split('\n');
			for( int j = 0; j < data_1.Length; j++ )
			{
				string[] row = data_1[j].Split(',');
				if( row.Length >= 15 )
				{
				if( row[13].Length > 0 )
					stars_with_planets_list.Add(row[13]);
				if( row[14].Length > 0 )
					stars_with_planets_list.Add (row[14]);
				if( row[15].Length > 0 )
					stars_with_planets_list.Add(row[15]);
				}
				else{ Debug.Log("row length: " + row.Length); break; }
			}
		}
		Debug.Log(stars_with_planets_list[4].ToString().Length);


		Star[] star_db;

		//Add stars to stars array
		string[] f_content = Resources.Load<TextAsset>("stars").text.Split('\n');
		star_db = new Star[f_content.Length - 1];

		int temptemp = 0;
		
		int debugcount = 0;
		int n = 1;
		for( int k = 0; k < f_content.Length - 2; k++ )
		{	n = k + 1;
			star_db[n] = new Star();

			string[] row = f_content[n].Split(',');

			if( row.Length >= 20 )
			{
				float.TryParse(row[7], out star_db[n].ra);
				float.TryParse(row[9], out star_db[n].dist);

				if( row[1].Length >= 0 )
				{ star_db[n].hipp = row[1]; }
				else
				{ star_db[n].hipp = ""; }

				
				float.TryParse(row[13], out star_db[n].posX);
				float.TryParse(row[14], out star_db[n].posY);
				float.TryParse(row[15], out star_db[n].posZ);

				if( temptemp >= 10000 )
				{
					temptemp = 0;
				}
				else
					temptemp++;

				star_db[n].luminosity = row[20];
				star_db[n].spectralType = row[12];
				if( star_db[n].spectralType.Length == 0 )
				{
					star_db[n].spectralType = "-";
				}
				star_db[n].constelation = row[16];

				if( row[6].Length > 0 )
				{
					star_db[n].name = row[6];
					star_db[n].nameLevel = 6;
				}
				else if(row[5].Length > 0 )
				{
					star_db[n].name = row[5];
					star_db[n].nameLevel = 5;
				}
				else if(row[4].Length > 0 )
				{
					star_db[n].name = row[4];
					star_db[n].nameLevel = 4;
				}
				else if(row[3].Length > 0 )
				{
					star_db[n].name = "hr" + row[3];
					star_db[n].nameLevel = 3;
				}
				else if(row[2].Length > 0 )
				{
					star_db[n].name = "HD " + row[2];
					star_db[n].nameLevel = 2;
				}
				else if( row[1].Length > 0 )
				{
					star_db[n].name = "hip" + row[1];
					star_db[n].nameLevel = 1;
				}
				else
				{
					star_db[n].name = row[0];
					star_db[n].nameLevel = 0;
				}




			}


			foreach( string str in stars_with_planets_list )
			{
				if( str == star_db[n].hipp.TrimEnd() )
				{
					star_db[n].has_planets = true;
					debugcount++;
				}
				else if( str == star_db[n].name.TrimEnd() )
				{
					star_db[n].has_planets = true;
					debugcount++;
				}
			}

			star_db[n].write(bw);

		}
		Debug.Log("matches found: " + debugcount);
		
		bw.Close();

		//bf.Serialize(fs, star_db);
		fs.Close();

		Debug.Log("done!");
	}
#endif

	public IEnumerator LoadStarsFromBinary()
	{
		Debug.Log("Started Load...");

		TextAsset file = Resources.Load("starDB") as TextAsset;

		//BinaryFormatter bf = new BinaryFormatter();
		//FileStream fs = File.Open("Assets/Resources/starInfo.dat", FileMode.Open);


		Stream fs = new MemoryStream(file.bytes);
		BinaryReader br = new BinaryReader(fs);
		stars = new Star[120000];
		int k = 0;
		for( int i = 0; i < 120000; i++ )
		{
			if( br.BaseStream.Position != br.BaseStream.Length )
			{
				stars[i] = new Star();
				stars[i].read(br);
			}
			else break;

			starsLength++;
			k++;
			if( k >= 10000 )
			{
				k = 0;
				loadingBar.value = (float)i/120000f;
				yield return null;
			}
		}
		fs.Close();

		loadingBar.gameObject.SetActive(false);
		loadingFinished = true;
		Debug.Log("Stars Loaded!");
		yield return null;
	}

	IEnumerator readStars(string filepath )
	{
		string[] f_content = starData.text.Split('\n');//System.IO.File.ReadAllLines(filepath);

		//Create an array of in-memory star data.  (1 row is the title)
		starsLength = f_content.Length - 1;
		stars = new Star[f_content.Length - 1];

		for( int n = 0; n < starsLength; n++ )
			stars[n] = new Star();


		loadingBar.value = 1.0f;

		int lbr = 0;  //Count to update the loading bar every 'lbr' records.
		for( int i = 0; i < starsLength; i++ )
		{
			string[] row = f_content[i+1].Split(',');


			//radial
			float ra = 0.0f;
			float dist = 0.0f;
			if( row.Length >= 20 )
			{
				float.TryParse(row[7], out ra);
				float.TryParse(row[9], out dist);


				///>>>  NEW STAR CATALOGUE TEST...
				/// 

				if( row[1].Length >= 0 )
				{ stars[i].hipp = row[1]; }
				else
				{ stars[i].hipp = ""; }

				float.TryParse(row[13], out stars[i].posX);
				float.TryParse(row[14], out stars[i].posY);
				float.TryParse(row[15], out stars[i].posZ);

				stars[i].ra = ra;
				stars[i].dist = dist;

				stars[i].luminosity = row[20];
				stars[i].spectralType = row[12];
				if( stars[i].spectralType.Length == 0 )
				{
					stars[i].spectralType = "-";
				}
				stars[i].constelation = row[16];

				if( row[6].Length > 0 )
				{
					stars[i].name = row[6];
					stars[i].nameLevel = 6;
				}
				else if(row[5].Length > 0 )
				{
					stars[i].name = row[5];
					stars[i].nameLevel = 5;
				}
				else if(row[4].Length > 0 )
				{
					stars[i].name = row[4];
					stars[i].nameLevel = 4;
				}
				else if(row[3].Length > 0 )
				{
					stars[i].name = "hr" + row[3];
					stars[i].nameLevel = 3;
				}
				else if(row[2].Length > 0 )
				{
					stars[i].name = "hd" + row[2];
					stars[i].nameLevel = 2;
				}
				else if( row[1].Length > 0 )
				{
					stars[i].name = "hip" + row[1];
					stars[i].nameLevel = 1;
				}
				else
				{
					stars[i].name = row[0];
					stars[i].nameLevel = 0;
				}

				///>>> END OF NEW CATALOGUE LOAD
				/// 
				/*
				if( dist < parsecLimit )
				{

					pos.x = Mathf.Cos (ra) * dist;
					pos.y = 0;
					pos.z = Mathf.Sin (ra) * dist;

					//switch(row[15].Substring(0,1))
					//{}

				//		GameObject ns = Instantiate(star, pos, Quaternion.identity) as GameObject;
				//		ns.transform.parent = transform;

					GameObject ns = new GameObject("star");
					MeshRenderer mr = ns.AddComponent<MeshRenderer>();
					MeshFilter mf = ns.AddComponent<MeshFilter>();
					mf.mesh = m;
					//MeshCollider mc = ns.AddComponent<MeshCollider>();
					//mc.convex = true;
					//mc.isTrigger = true;
					mr.material = mG;

					ns.transform.position = pos;

					if( row[12].Length > 0 )
					{
						char type = row[12][0];

						if( type == 'G' )
							mr.material = mG; // = new Color(225, 159, 100);
						else if(type == 'A')
							mr.material = mA;
						else if(type == 'B')
							mr.material = mB;
						else if(type == 'F')
							mr.material = mF;
						else if(type == 'K')
							mr.material = mK;
						else if(type == 'M')
							mr.material = mM;
						else
							mr.material = mG;


					}

					//Text t = Instantiate(starText) as Text;
					//t.rectTransform.parent = canvas.transform;

					StarBehaviour behaviour = ns.AddComponent<StarBehaviour>();
					if( behaviour != null )
					{
						if( row[6].Length > 0 )
						{
							behaviour.starName = row[6];
							behaviour.nameLevel = 6;
						}
						else if(row[5].Length > 0 )
						{
							behaviour.starName = row[5];
							behaviour.nameLevel = 5;
						}
						else if(row[4].Length > 0 )
						{
							behaviour.starName = row[4];
							behaviour.nameLevel = 4;
						}
						else if(row[3].Length > 0 )
						{
							behaviour.nameLevel = 3;
							behaviour.starName = "hr" + row[3];
						}
						else if(row[2].Length > 0 )
						{
							behaviour.nameLevel = 2;
							behaviour.starName = "hd" + row[2];
						}
						else if( row[1].Length > 0 )
						{
							behaviour.nameLevel = 1;
							behaviour.starName = "hip" + row[1];
						}
						else
						{
							behaviour.nameLevel = 0;
							behaviour.starName = row[0];
						}

						//3D coords
						float.TryParse(row[13], out behaviour.pos.x);
						float.TryParse(row[14], out behaviour.pos.y);
						float.TryParse(row[15], out behaviour.pos.z);

						behaviour.spectralType = row[12];

						behaviour.constelation = row[16];

						if( row[1].Length > 0 )
						{
							behaviour.hip = row[1];
						}
						else behaviour.hip = "missing";
						float.TryParse(row[20], out behaviour.luminosity);

						behaviour.ra = ra;
						behaviour.dist = dist;

						behaviour.t_toCopy = t_toCopy;
					}

				}*/
			}

			loadingBar.value = (float)i / f_content.Length;
			if( lbr == 10000 )
			{
				lbr = 0;
				yield return null;
			}
			else lbr++;
		}
		loadingBar.gameObject.SetActive(false);
		loadingFinished = true;


		yield return null;
	}
}
