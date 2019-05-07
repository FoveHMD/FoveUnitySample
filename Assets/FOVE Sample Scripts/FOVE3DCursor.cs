using UnityEngine;

public class FOVE3DCursor : FOVEBehavior
{
	public enum LeftOrRight
	{
		Left,
		Right
	}

	[SerializeField]
	public LeftOrRight whichEye;

	// Use this for initialization
	void Start ()
    {
    }

	// Latepdate ensures that the object doesn't lag behind the user's head motion
	void Update()
    {
		var rays = FoveInterface.GetGazeRays();
		var ray = whichEye == LeftOrRight.Left ? rays.left : rays.right;

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
