using UnityEngine;
using System.Collections;

public class Task : MonoBehaviour {
	
	public int level;
	public int score;
	public int distance;
	public int kill;
	public int headshot;
	public int[] badge;
	public Sprite rank;
	public string achieveId;

	// Use this for initialization
	void Start () {
		/*description = level + ".\n";
		if(distance > 0)
			description += string.Format("Move %dm\n", distance);
		if(kill > 0)
			description += string.Format("Kill %d Enemies\n", distance);*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
}
