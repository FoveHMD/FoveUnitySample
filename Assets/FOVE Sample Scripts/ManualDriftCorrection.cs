using UnityEngine;
using System.Collections;

public class ManualDriftCorrection : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			this.GetComponent<Renderer>().enabled = true;
		}

		if (this.GetComponent<Renderer>().enabled)
		{
			RaycastHit hit;
			FoveInterface foveInterface = transform.parent.GetComponent<FoveInterface>();
			Ray ray = new Ray(foveInterface.transform.position, foveInterface.transform.forward);
			if (Physics.Raycast(ray, out hit, 10.0f))
			{
				transform.position = hit.point;
			}
			else
			{
				transform.position = foveInterface.transform.position + foveInterface.transform.forward * 1.5f;
			}
		}

		if (Input.GetKeyUp(KeyCode.M))
		{
			this.GetComponent<Renderer>().enabled = false;
			Vector3 loc = transform.localPosition;
			Fove.Managed.SFVR_Vec3 driftLocation;
			driftLocation.x = loc.x;
			driftLocation.y = loc.y;
			driftLocation.z = loc.z;
			FoveInterfaceBase.GetFVRHeadset().ManualDriftCorrection3D(driftLocation);
		}
	}
}
