using UnityEditor;
using UnityEngine;

namespace FoveSettings
{
	public abstract class SuggestedProjectFix
	{
		public string Description;
		public string HelpText;
		public void Fix(FoveSettings settings)
		{
			m_LastCheckTime = 0;
			RealFix(settings);
		}

		protected float m_RecheckInterval = 0.1f;

		private bool m_CachedCheck = false;
		private double m_LastCheckTime;
		public bool IsOkay(FoveSettings settings, bool force = false) // return false if the suggestion applies
		{
			double t = EditorApplication.timeSinceStartup;
			if (force || t - m_LastCheckTime > m_RecheckInterval)
			{
				m_CachedCheck = RealCheck(settings);
				m_LastCheckTime = t;
			}

			return m_CachedCheck;
		}

		protected abstract bool RealCheck(FoveSettings settings);
		protected abstract void RealFix(FoveSettings settings);
	}

	// Require that VR be enabled when using the experimental FoveInterface2 setting
	public class RequireVR_Suggestion : SuggestedProjectFix
	{
		public RequireVR_Suggestion()
		{
			Description = "VR is not enabled";
			HelpText = "The selected FOVE interface requires VR to be enabled in your project settings " +
				"in order to function. This allows it to take advantage of Unity's internal VR optimisations." +
				"\n\n" +
				"Be aware that this setting is experimental. There may be some performance " +
				"issues and you may see Unity-engine warnings or errors reported.";
		}

		protected override bool RealCheck(FoveSettings settings)
		{
			return PlayerSettings.virtualRealitySupported || settings.interfaceChoice != InterfaceChoice.VrRenderPath;
		}

		protected override void RealFix(FoveSettings settings)
		{
			PlayerSettings.virtualRealitySupported = true;

			var splitDevice_Requirement = new RequireSplitVrDevice_Suggestion();
			if (!splitDevice_Requirement.IsOkay(settings, true))
			{
				splitDevice_Requirement.Fix(settings);
			}
		}
	}

	// Require that the "split" stereo device exist when using the experimental FoveInterface2
	public class RequireSplitVrDevice_Suggestion : SuggestedProjectFix
	{
		public RequireSplitVrDevice_Suggestion()
		{
			Description = "FOVE needs the \"split\" VR device";
			HelpText = "FOVE uses the built-in \"Split Stereo Display (non head-mounted)\" VR device to take " +
				"advantage of Unity's stereo-rendering optimzations, even though it claims to not be for " +
				"head-mounted displays." +
				"\n\n" +
				"Your selected FOVE interface requires that this device be present in your VR device list.";
			m_RecheckInterval = 3.0f;
		}

		// Helper function to keep the call site cleaner now that we need the version #ifs
		private string[] GetSupportedDevices()
		{
#if UNITY_2017_2_OR_NEWER
			string[] devices = UnityEngine.XR.XRSettings.supportedDevices;
#else
			string[] devices = UnityEngine.VR.VRSettings.supportedDevices;
#endif
			return devices;
		}

		protected override bool RealCheck(FoveSettings settings)
		{
			string[] devices = GetSupportedDevices();
			if (settings.interfaceChoice != InterfaceChoice.VrRenderPath)
				return true;
			if (PlayerSettings.virtualRealitySupported != true)
				return true; // we cannot determine if this suggestion applies when VR is disabled

			for (int i = 0; i < devices.Length; i++)
			{
				if (devices[i] == "split")
				{
					return true;
				}
			}

			return false;
		}

		protected override void RealFix(FoveSettings settings)
		{
			string[] devices = GetSupportedDevices();
			string[] new_devices;

			bool skipCopy = false;
			if (devices.Length == 1 && devices[0] == "None")
			{
				new_devices = new string[1];
				skipCopy = true;
			}
			else
			{
				new_devices = new string[devices.Length + 1];
			}

			int i = 1;
			new_devices[0] = "split";
			if (!skipCopy)
			{
				foreach (var device in devices)
				{
					new_devices[i++] = device;
				}
			}

#if UNITY_5_5_OR_NEWER
				UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.Standalone, new_devices);
#else
				UnityEditorInternal.VR.VREditor.SetVREnabledDevices(BuildTargetGroup.Standalone, new_devices);
#endif
		}
	}

	// Require that VR be disabled when using the stable FoveInterface
	public class RequireNoVr_Suggestion : SuggestedProjectFix
	{
		public RequireNoVr_Suggestion()
		{
			Description = "VR should not be enabled";
			HelpText = "The selected FOVE interface works best when VR is disabled, and having VR enabled could " +
				"cause performance problems and graphical anomalies.";
		}

		protected override bool RealCheck(FoveSettings settings)
		{
			return !PlayerSettings.virtualRealitySupported || settings.interfaceChoice != InterfaceChoice.DualCameras;
		}

		protected override void RealFix(FoveSettings settings)
		{
			PlayerSettings.virtualRealitySupported = false;
		}
	}

	// Require that Vsync be disabled
	public class RequireVsyncOff_Suggestion : SuggestedProjectFix
	{
		public RequireVsyncOff_Suggestion()
		{
			Description = "Vsync should be disabled";
			HelpText = "One or more of your Quality Settings has vsync on. The FOVE interface will automatically " +
				"manage the refresh rate of your scene to match the headset, which means that if you have Vsync " +
				"enabled is likely to disrupt smooth performance in VR." +
				"\n\n" +
				"We recommend disabling Vsync in all quality settings to ensure the best possible experience for " +
				"your users.";
			m_RecheckInterval = 0.7f;
		}

		protected override bool RealCheck(FoveSettings settings)
		{
			string[] qualityNames = QualitySettings.names;
			for (int i = 0; i < qualityNames.Length; i++)
			{
				QualitySettings.SetQualityLevel(i);
				if (QualitySettings.vSyncCount > 0)
				{
					return false;
				}
			}

			return true;
		}

		protected override void RealFix(FoveSettings settings)
		{
			string[] qualityNames = QualitySettings.names;
			for (int i = 0; i < qualityNames.Length; i++)
			{
				QualitySettings.SetQualityLevel(i);
				QualitySettings.vSyncCount = 0;
			}
		}
	}

	// Require Windows 64-bit build settings
	public class RequireWin64Bit_Suggestion : SuggestedProjectFix
	{
		public RequireWin64Bit_Suggestion()
		{
			Description = "Set build target to Windows 64-bit";
			HelpText = "The FOVE plugin currently only supports Windows 64-bit builds. Anything else will fail " +
				"to load our libraries and won't run properly.";
			m_RecheckInterval = 0.77f;
		}

		protected override bool RealCheck(FoveSettings settings)
		{
			return EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64;
		}

		protected override void RealFix(FoveSettings settings)
		{
			bool success = false;
#if UNITY_5_6_OR_NEWER
				success = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
#else
				success = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64);
#endif

			if (!success)
			{
				EditorUtility.DisplayDialog("Error Changing Build Settings",
					"Switch settings function returned false...", "Okay");
			}
		}
	}
}