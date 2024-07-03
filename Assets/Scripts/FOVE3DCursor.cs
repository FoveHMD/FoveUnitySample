using UnityEngine;

public class FOVE3DCursor : FOVEBehavior
{
	[SerializeField]
	public Fove.Eye whichEye;

	void Update()
	{
		var result = FoveInterface.GetGazeRay(whichEye);
		var err = result.error;
		if (err == Fove.ErrorCode.License_FeatureAccessDenied)
		{
			result = FoveInterface.GetCombinedGazeRay();
		}

		var ray = result.value;

		RaycastHit hit;
		Physics.Raycast(ray, out hit, Mathf.Infinity);
		if (hit.point != Vector3.zero) // Vector3 is non-nullable; comparing to null is always false
		{
			transform.position = hit.point;
		}
		else
		{
			transform.position = ray.GetPoint(3.0f);
		}
	}
}
