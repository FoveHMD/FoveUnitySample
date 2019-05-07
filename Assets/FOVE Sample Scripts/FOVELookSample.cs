using Fove.Unity;
using UnityEngine;

public class FOVELookSample : FOVEBehavior
{
    public Light attachedLight;

	private Collider my_collider;
	private Material material;
	private bool light_attached = false;
	
	void Start()
    {
		my_collider = GetComponent<Collider>();

		if (attachedLight == null)
			attachedLight = transform.GetComponentInChildren<Light>();

		if (attachedLight)
		{
			light_attached = true;
			attachedLight.enabled = false;
		}
		material = gameObject.GetComponent<Renderer>().material;

		if (material == null)
			gameObject.SetActive(false);
	}
	
	void Update ()
    {
		if (FoveInterface.Gazecast(my_collider))
		{
			material.EnableKeyword("_EMISSION");

			if (light_attached)
			{
				material.SetColor("_EmissionColor", attachedLight.color);
				attachedLight.enabled = true;
				DynamicGI.SetEmissive(GetComponent<Renderer>(), attachedLight.color);
			}
		}
		else
		{
			gameObject.GetComponent<Renderer> ().material.color = Color.white;
			material.DisableKeyword("_EMISSION");

			if (light_attached)
			{
				attachedLight.enabled = false;
				DynamicGI.SetEmissive(GetComponent<Renderer>(), Color.black);
			}
		}
	}
}
