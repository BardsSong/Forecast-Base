using UnityEditor;
using UnityEngine;

public class WorldSimulationWindow : EditorWindow
{
	bool gravityGroup = false;
	bool timeGroup = false;
	float gravityValue = -9.81f;
	float maximumGravity = 50f;
	float minimumGravity = -50f;
	bool allowChangeGravity = false;
	bool allowTimeScaleChange = false;
	float maximumTimeScale = 5.0f;
	float timeScale = 1.0f;

	public Object rainEffect = new Object();
	public Object snowEffect = new Object();
	public Object cloudEffect = new Object();
	Object currentWeather = new Object();

	WeatherType currentWeatherType = new WeatherType();

	[MenuItem("Window/World Window")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(WorldSimulationWindow));
	}

	void OnGUI()
	{
		Weather[] allWeather = (Weather[])Resources.FindObjectsOfTypeAll(typeof(Weather));

		GUILayout.Label("World Settings", EditorStyles.boldLabel);

		gravityGroup = EditorGUILayout.BeginToggleGroup("Gravity", gravityGroup);
			minimumGravity = EditorGUILayout.FloatField("Minimum Gravity", minimumGravity);
			maximumGravity = EditorGUILayout.FloatField("Maximum Gravity", maximumGravity);
			gravityValue = EditorGUILayout.Slider("Gravity", gravityValue, minimumGravity, maximumGravity);
			allowChangeGravity = EditorGUILayout.Toggle("Allow Gravity to Change", allowChangeGravity);
		EditorGUILayout.EndToggleGroup();

		if (allowChangeGravity)
		{
			Physics.gravity = new Vector3(0.0f, gravityValue, 0.0f);
		}

		timeGroup = EditorGUILayout.BeginToggleGroup("Time", timeGroup);
			allowTimeScaleChange = EditorGUILayout.Toggle("Allow Time Scale Change", allowTimeScaleChange);
			maximumTimeScale = EditorGUILayout.FloatField("Maximum Time Scale", maximumTimeScale);
			timeScale = EditorGUILayout.Slider("Time Scale", timeScale, 0.0f, maximumTimeScale);
		EditorGUILayout.EndToggleGroup();

		if (allowTimeScaleChange)
		{
			Time.timeScale = timeScale;
		}

		GUILayout.Label("Weather Settings", EditorStyles.boldLabel);

		rainEffect = EditorGUILayout.ObjectField("Rain Effect", rainEffect, typeof(GameObject), false);
		snowEffect = EditorGUILayout.ObjectField("Snow Effect", snowEffect, typeof(GameObject), false);
		cloudEffect = EditorGUILayout.ObjectField("Cloud Effect", cloudEffect, typeof(GameObject), false);

		currentWeather = EditorGUILayout.ObjectField("Current Weather Object", (Selection.activeGameObject != null && Selection.activeGameObject.tag == "WeatherObject") ? Selection.activeGameObject : currentWeather, typeof(GameObject), false);

		if (currentWeather != null)
		{
			GameObject useableObject = (GameObject)currentWeather;
			Weather weatherComponent = useableObject.GetComponent<Weather>();
			currentWeatherType = (WeatherType)EditorGUILayout.EnumPopup("Current Weather", currentWeatherType);
			weatherComponent.ChangeWeather(currentWeatherType);

			
		}

		if (rainEffect)
		{
			foreach (Weather item in allWeather)
			{
				item.RainBase = (GameObject)rainEffect;
			}
		}

		if (cloudEffect)
		{
			foreach (Weather item in allWeather)
			{
				item.CloudBase = (GameObject)cloudEffect;
			}
		}

		if (snowEffect)
		{
			foreach (Weather item in allWeather)
			{
				item.SnowBase = (GameObject)snowEffect;
			}
		}
	}
}
