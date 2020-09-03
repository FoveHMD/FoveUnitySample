using Fove.Unity;
using UnityEngine;

public class FoveControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		FoveManager.TareOrientation();
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.C))
			FoveManager.StartEyeTrackingCalibration();

		if (Input.GetKeyDown(KeyCode.T))
			FoveManager.TareOrientation();
    }
}
