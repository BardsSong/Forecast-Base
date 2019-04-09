using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeatherManager : MonoBehaviour {

	List<Weather> weatherObjects = new List<Weather>();

	void Start () {
		List<GameObject> genericObjects = new List<GameObject>();
		genericObjects.AddRange(GameObject.FindGameObjectsWithTag("WeatherObject"));

		foreach (GameObject item in genericObjects)
		{
			weatherObjects.Add(item.GetComponent<Weather>());
		}
	}

}
