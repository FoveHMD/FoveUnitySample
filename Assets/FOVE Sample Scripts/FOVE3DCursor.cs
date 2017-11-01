using UnityEngine;
using System.Collections;

public class FOVE3DCursor : MonoBehaviour
{
	public enum LeftOrRight
	{
		Left,
		Right
	}

	[SerializeField]
	public LeftOrRight whichEye;
	public FoveInterfaceBase foveInterface;

	// Use this for initialization
	void Start () {
	}

	// Latepdate ensures that the object doesn't lag behind the user's head motion
	void Update() {
		FoveInterfaceBase.EyeRays rays = foveInterface.GetGazeRays();

		Ray r = whichEye == LeftOrRight.Left ? rays.left : rays.right;

		RaycastHit hit;
		Physics.Raycast(r, out hit, Mathf.Infinity);
		if (hit.point != Vector3.zero) // Vector3 is non-nullable; comparing to null is always false
		{
			transform.position = hit.point;
		}
		else
		{
			transform.position = r.GetPoint(3.0f);
		}
	}
}
