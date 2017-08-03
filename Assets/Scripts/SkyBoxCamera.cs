using UnityEngine;

namespace Assets.Scripts
{
	public class SkyBoxCamera : MonoBehaviour
	{
		public Camera MainCamera;
		public Camera SkyCamera;

		public Vector3 SkyBoxRotation;

		void Start()
		{
			if (SkyCamera.depth >= MainCamera.depth)
			{
				Debug.Log("Set skybox camera depth lower than main camera depth in inspector");
			}
			if (MainCamera.clearFlags != CameraClearFlags.Nothing)
			{
				Debug.Log("Main camera needs to be set to dont clear in the inspector");
			}
		}

		public void SetSkyBoxRotation(Vector3 rotation)
		{
			this.SkyBoxRotation = rotation;
		}

		void Update()
		{
			SkyCamera.transform.position = MainCamera.transform.position;
			SkyCamera.transform.rotation = MainCamera.transform.rotation;
			SkyCamera.transform.Rotate(SkyBoxRotation);
		}
	}
}