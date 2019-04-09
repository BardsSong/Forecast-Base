using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Weather : MonoBehaviour {

	[SerializeField] bool AllowChangeWeather = true;
	public WeatherType previousWeather;
	public WeatherType currentWeather = WeatherType.Sun;
	public WeatherType startingWeather = WeatherType.Rain;

	[SerializeField] float weatherDuration = 0.0f;
	[SerializeField] public float currentWeatherTime = 0.0f;
	public List<GameObject> LandscapeObjects = new List<GameObject>();

	[SerializeField] float NormalStaticFriction = 0.6f;
	[SerializeField] float NormalDynamicFriction = 0.6f;
	[SerializeField] float RainyStaticFriction = 0.6f;
	[SerializeField] float RainyDynamicFriction = 0.3f;
	[SerializeField] float SnowStaticFriction = 0.0f;
	[SerializeField] float SnowDynamicFriction = 0.0f;

	[SerializeField] int NormalTemperature = 72;
	[SerializeField] int RainTemperature = 50;
	[SerializeField] int SnowTemperature = 20;

	[SerializeField] Vector3 RainEffectPosition;
	[SerializeField] Vector3 SnowEffectPosition;
	[SerializeField] Vector3 CloudEffectPosition;

	public GameObject RainBase = null;
	public GameObject SnowBase = null;
	public GameObject CloudBase = null;

	private GameObject RainA = null;
	private GameObject SnowA = null;
	private GameObject CloudA = null;
	private GameObject RainB = null;
	private GameObject SnowB = null;
	private GameObject CloudB = null;

	private ParticleSystem RainParticleA;
	private ParticleSystem SnowParticleA;
	private ParticleSystem CloudParticleA;
	private ParticleSystem RainParticleB;
	private ParticleSystem SnowParticleB;
	private ParticleSystem CloudParticleB;

	[SerializeField] int RainPercentage = 35;
	[SerializeField] int SnowPercentage = 35;
	[SerializeField] int CloudPercentage = 20;
	[SerializeField] int ClearPercentage = 10;

	[SerializeField] int RainEmissionRate = 1000;
	[SerializeField] int SnowEmissionRate = 100;
	[SerializeField] int CloudEmissionRate = 500;

	[SerializeField] bool SwappingAB;
	[SerializeField] bool SwappingBA;


	private void Start()
	{
		this.tag = "WeatherObject";
		currentWeather = startingWeather;

		SetWeatherObjects();

		if (RainA != null)
		{
			RainParticleA = RainA.GetComponent<ParticleSystem>();
		}

		if (RainB != null)
		{
			RainParticleB = RainB.GetComponent<ParticleSystem>();
		}

		if (SnowA != null)
		{
			SnowParticleA = SnowA.GetComponent<ParticleSystem>();
		}

		if (SnowB != null)
		{
			SnowParticleB = SnowB.GetComponent<ParticleSystem>();
		}

		if (CloudA != null)
		{
			CloudParticleA = CloudA.GetComponent<ParticleSystem>();
		}

		if (CloudB != null)
		{
			CloudParticleB = CloudB.GetComponent<ParticleSystem>();
		}

		PauseEverything();
		ChangeWeatherEffect();
		ChangeEffectedObjects();
	}

	private void SetWeatherObjects()
	{
		RainA = Instantiate(RainBase, this.transform);
		RainA.transform.localPosition = RainEffectPosition;
		RainB = Instantiate(RainBase, this.transform);
		RainB.transform.localPosition = RainEffectPosition;

		SnowA = Instantiate(SnowBase, this.transform);
		SnowA.transform.localPosition = SnowEffectPosition;
		SnowB = Instantiate(SnowBase, this.transform);
		SnowB.transform.localPosition = SnowEffectPosition;

		CloudA = Instantiate(CloudBase, this.transform);
		CloudA.transform.localPosition = CloudEffectPosition;
		CloudB = Instantiate(CloudBase, this.transform);
		CloudB.transform.localPosition = CloudEffectPosition;
	}

	private void PauseEverything()
	{
		RainParticleA.Stop(false, ParticleSystemStopBehavior.StopEmitting);
		RainParticleB.Stop(false, ParticleSystemStopBehavior.StopEmitting);
		SnowParticleA.Stop(false, ParticleSystemStopBehavior.StopEmitting);
		SnowParticleB.Stop(false, ParticleSystemStopBehavior.StopEmitting);
		CloudParticleA.Stop(false, ParticleSystemStopBehavior.StopEmitting);
		CloudParticleB.Stop(false, ParticleSystemStopBehavior.StopEmitting);
	}

	private void Update()
	{
		currentWeatherTime += Time.deltaTime;
		if (NeedsWeatherChange() && AllowChangeWeather)
		{
			ChangeWeather();
		}
	}

	public bool NeedsWeatherChange()
	{
		if (currentWeatherTime >= weatherDuration)
		{
			return true;
		}
		return false;
	}

	public void ChangeWeather()
	{
		currentWeatherTime = 0.0f;
		int weatherChosen = UnityEngine.Random.Range(0, 100);
		previousWeather = currentWeather;

		if (weatherChosen >= 0.0f && weatherChosen <= RainPercentage)
		{
			currentWeather = WeatherType.Rain;
		}
		else if (weatherChosen >= RainPercentage +1 && weatherChosen <= RainPercentage + SnowPercentage)
		{
			currentWeather = WeatherType.Snow;
		}
		else if (weatherChosen >= RainPercentage + SnowPercentage + 1 && weatherChosen <= RainPercentage + SnowPercentage + CloudPercentage)
		{
			currentWeather = WeatherType.Cloud;
		}
		else if (weatherChosen >= RainPercentage + SnowPercentage + CloudPercentage + 1 && weatherChosen <= 100)
		{
			currentWeather = WeatherType.Sun;
		}

		ChangeWeatherEffect();
		StartCoroutine("WeatherSwap");
		ChangeEffectedObjects();
	}

	public void ChangeWeather(WeatherType weather)
	{
		currentWeatherTime = 0.0f;
		previousWeather = currentWeather;

		currentWeather = weather;
		ChangeWeatherEffect();
		StartCoroutine("WeatherSwap");
		ChangeEffectedObjects();
	}

	public void ChangeWeatherEffect()
	{
		if ((!SwappingAB && !SwappingBA)|| SwappingAB)
		{
			SwappingAB = false;
			SwappingBA = true;
		}
		else
		{
			SwappingBA = false;
			SwappingAB = true;
		}

		switch (currentWeather)
		{
			case WeatherType.Sun:
				if (SwappingAB)
				{
					RainParticleB.Stop();
					SnowParticleB.Stop();
					CloudParticleB.Stop();
				}
				else
				{
					RainParticleA.Stop();
					SnowParticleA.Stop();
					CloudParticleA.Stop();
				}
				break;
			case WeatherType.Cloud:
				if (SwappingAB)
				{
					RainParticleB.Stop();
					SnowParticleB.Stop();
					CloudParticleB.Play();
				}
				else
				{
					RainParticleA.Stop();
					SnowParticleA.Stop();
					CloudParticleA.Play();
				}
				break;
			case WeatherType.Rain:
				if (SwappingAB)
				{
					RainParticleB.Play();
					SnowParticleB.Stop();
					CloudParticleB.Play();
				}
				else
				{
					RainParticleA.Play();
					SnowParticleA.Stop();
					CloudParticleA.Play();
				}
				break;
			case WeatherType.Snow:
				if (SwappingAB)
				{
					RainParticleB.Stop();
					SnowParticleB.Play();
					CloudParticleB.Play();
				}
				else
				{
					RainParticleA.Stop();
					SnowParticleA.Play();
					CloudParticleA.Play();
				}
				break;
		}
	}

	public void ChangeEffectedObjects()
	{
		foreach (GameObject item in LandscapeObjects)
		{
			Collider other = item.GetComponent<Collider>();

			PhysicMaterial slippery = null;
			if (other.material != null && other.material.name == "Landscape Effector (Instance)")
			{
				slippery = other.material;
			}
			else
			{
				slippery = new PhysicMaterial();
				slippery.name = "Landscape Effector";
			}

			switch (this.GetComponentInParent<Weather>().currentWeather)
			{
				case WeatherType.Sun:
					slippery.staticFriction = NormalStaticFriction;
					slippery.dynamicFriction = NormalDynamicFriction;
					break;
				case WeatherType.Cloud:
					slippery.staticFriction = NormalStaticFriction;
					slippery.dynamicFriction = NormalDynamicFriction;
					break;
				case WeatherType.Rain:
					slippery.staticFriction = RainyStaticFriction;
					slippery.dynamicFriction = RainyDynamicFriction;
					break;
				case WeatherType.Snow:
					slippery.staticFriction = SnowStaticFriction;
					slippery.dynamicFriction = SnowDynamicFriction;
					break;

			}


			other.material = slippery;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Landscape" && !LandscapeObjects.Contains(other.gameObject))
		{
			LandscapeObjects.Add(other.gameObject);
		}

		ChangeEffectedObjects();
	}

	IEnumerator WeatherSwap()
	{
		for (float i = 0.0f; i < 110; i+=10)
		{
			if (SwappingAB)
			{
				ParticleSystem.EmissionModule emissionModule;

				if (RainA.activeSelf)
				{
					emissionModule = RainParticleA.emission;
					emissionModule.rateOverTime = ((100.0f - i) / 100.0f) * RainEmissionRate;
				}

				if (SnowA.activeSelf)
				{
					emissionModule = SnowParticleA.emission;
					emissionModule.rateOverTime = ((100.0f - i) / 100.0f) * SnowEmissionRate;
				}

				if (CloudA.activeSelf)
				{
					emissionModule = CloudParticleA.emission;
					emissionModule.rateOverTime = ((100.0f - i) / 100.0f) * CloudEmissionRate;
				}
				

				if (RainB.activeSelf)
				{
					emissionModule = RainParticleB.emission;
					emissionModule.rateOverTime = (i / 100.0f) * RainEmissionRate;
				}

				if (SnowB.activeSelf)
				{
					emissionModule = SnowParticleB.emission;
					emissionModule.rateOverTime = (i / 100.0f) * SnowEmissionRate;
				}

				if (CloudB.activeSelf)
				{
					emissionModule = CloudParticleB.emission;
					emissionModule.rateOverTime = (i / 100.0f) * CloudEmissionRate;
				}
			}
			else if(SwappingBA)
			{
				ParticleSystem.EmissionModule emissionModule;
				if (RainA.activeSelf)
				{
					emissionModule = RainParticleA.emission;
					emissionModule.rateOverTime = (i / 100.0f) * RainEmissionRate;
				}

				if (SnowA.activeSelf)
				{
					emissionModule = SnowParticleA.emission;
					emissionModule.rateOverTime = (i / 100.0f) * SnowEmissionRate;
				}

				if (CloudA.activeSelf)
				{
					emissionModule = CloudParticleA.emission;
					emissionModule.rateOverTime = (i / 100.0f) * CloudEmissionRate;
				}
				
				if (RainB.activeSelf)
				{
					emissionModule = RainParticleB.emission;
					emissionModule.rateOverTime = ((100.0f - i) / 100.0f) * RainEmissionRate;
				}

				if (SnowB.activeSelf)
				{
					emissionModule = SnowParticleB.emission;
					emissionModule.rateOverTime = ((100.0f - i) / 100.0f) * SnowEmissionRate;
				}

				if (CloudB.activeSelf)
				{
					emissionModule = CloudParticleB.emission;
					emissionModule.rateOverTime = ((100.0f - i) / 100.0f) * CloudEmissionRate;
				}
			}
			
			yield return new WaitForSeconds(.1f);
		}
		DisableSystems();
	}

	private void DisableSystems()
	{
		if (SwappingAB)
		{
			RainParticleA.Stop();
			SnowParticleA.Stop();
			CloudParticleA.Stop();
			
		}
		else
		{
			RainParticleB.Stop();
			SnowParticleB.Stop();
			CloudParticleB.Stop();
		}
	}
}
