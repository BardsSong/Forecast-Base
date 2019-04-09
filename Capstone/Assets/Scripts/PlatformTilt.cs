using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTilt : MonoBehaviour {

	Rigidbody thisBody;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Untagged")
		{
			thisBody = GetComponent<Rigidbody>();
			StartCoroutine("RotatePls");
		}
	}

	IEnumerator RotatePls()
	{
		for (int i = 0; i < 45; i++)
		{
			this.transform.Rotate(new Vector3(1.0f, 0.0f, 0.0f), 1.0f);
			yield return new WaitForSeconds(.05f);
		}
	}
}
