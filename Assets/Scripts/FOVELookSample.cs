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
		material = GetComponent<Renderer>().material;

		if (material == null)
			gameObject.SetActive(false);
	}
	
	void Update ()
    {
		var isGazed = FoveSettings.AutomaticObjectRegistration
			? FoveManager.GetGazedObject() == gameObject // use the Object detection API
			: FoveInterface.Gazecast(my_collider); // Manually perform gaze cast on scene colliders

		Color emissiveColor;

		if (isGazed)
		{
			emissiveColor = attachedLight.color;
			material.EnableKeyword("_EMISSION");
		}
		else
		{
			emissiveColor = Color.black;
			material.DisableKeyword("_EMISSION");
		}

		if (light_attached)
		{
			attachedLight.enabled = isGazed;
			DynamicGI.SetEmissive(GetComponent<Renderer>(), emissiveColor);
			material.SetColor("_EmissionColor", attachedLight.color);
		}
	}
}
