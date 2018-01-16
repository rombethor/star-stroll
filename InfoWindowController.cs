using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoWindowController : MonoBehaviour {

	public CameraBehaviour camBeh;

	public StarBehaviour sb;

	public Star star;

	public Text textName;
	public Text textSpectralType;
	public Text textDistance;
	public Text textConstelation;
	public Text textLuminosity;

	public Text textPlanetCount;
	public Text textPlanetInfo;

	public TextAsset planetData;

	//test
	public string hipp;

	// Use this for initialization
	void Start () {
		if( planetData == null )
			planetData = Resources.Load<TextAsset>("exoplanets");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable(){
		LoadInformation();
	}

	public void LoadInformation()
	{
		if( camBeh.target != null ) sb = camBeh.target.GetComponent<StarBehaviour>(); else Debug.Log("Null target!");
		if( sb != null ) star = sb.star; else Debug.Log ("Star behaviour was null!");
		if( star != null )
		{
			textName.text = star.name;
			textDistance.text = "Distance: " + star.dist + "pc";
			textSpectralType.text = "Spectral Type: " + star.spectralType;
			textLuminosity.text = "Luminosity: " + star.luminosity + "suns";
			textConstelation.text = "Constelation: " + star.constelation;


			if( star.planetTextLoaded )
			{
				textPlanetInfo.text = star.planetText;
				textPlanetCount.text = "Known Planets: " + star.planetCount;
				textPlanetInfo.rectTransform.sizeDelta = new Vector2(textPlanetInfo.rectTransform.rect.width, star.planetCount*210);
			}
			else
			{
				if( star.has_planets )
				{
					if( star.hipp != null && star.hipp.Length > 0 )
					{
						StartCoroutine(loadPlanets ());
					}
					else
					{

						textPlanetCount.text = "Known Planets: 0";
						textPlanetInfo.text = "";
					}
				}
				else
				{
					textPlanetCount.text = "Known Planets: 0";
					textPlanetInfo.text = "";
				}
			}
		}
		else Debug.Log ("StarBehaviour.star was null!");
	}

	IEnumerator loadPlanets()
	{

		if( planetData.text.Length <= 0 )
			Debug.Log("Planet Data is missing!");

		string [] f_content = planetData.text.Split('\n');//System.IO.File.ReadAllLines(Application.persistentDataPath + "/exoplanets.csv");


		star.planetText = "\r";
		textPlanetInfo.text = "Loading...";
		textPlanetCount.text = "Known Planets: ...";

		float height = 20;


		int count = 0;
		for( int i = 0; i < f_content.Length; i++ )
		{
			string[] row = f_content[i].Split(',');


			if( row.Length >= 15 )
			{
				if( row[15].Length > 0 && star.hipp.TrimEnd().Length > 0 )
				{
					if( row[15].TrimEnd() == star.hipp.TrimEnd() || row[13] == star.name.TrimEnd() ||  row[14] == star.name.TrimEnd() )
					{
						star.planetText += row[0] + "\nClass: " + row[2] + " " + row[1] + "\nComposition: " + row[3] + "\nAtmosphere: " + row[4] + "\nMass: " + row[6] + "earths\nRadius: " + row[7] + "earth radii\nRelative Density: " + row[8] + "\nGravity: " + row[9] + "g\nOrbital Period: " + row[10] + "days\nOrbital Distance: " + row[12] + "au\nEccentricity: " + row[11] + "\nHabitable: " + row[5] + "\n\n";
						count++;
						height += 270;
						yield return null;
					}
				}

			}

		}

		star.planetCount = count;
		star.planetTextLoaded = true;
		textPlanetInfo.rectTransform.sizeDelta = new Vector2(textPlanetInfo.rectTransform.rect.width, height);
		textPlanetInfo.text = star.planetText;
		textPlanetCount.text = "Known Planets: " + count;
		yield return null;
	}

	public void ButtonExit()
	{
		CameraBehaviour.guiTouch = true;

		gameObject.SetActive(false);
	}
}
