using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Orbit a camera from passed input value, ideally feed input value with device input. Performed every frame and per  second" +
"Set inputX or inputY to None if you want to keep these value inchanged")]
	public class CameraOrbit : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;
		[Tooltip("in landscape condition for iOS invert X and Y")]
		public FsmFloat inputRotationX;
		public FsmFloat inputRotationY;
		public FsmFloat sensitivityX;
		public FsmFloat sensitivityY;
		public FsmBool clampX;
		public FsmFloat xMin;
		public FsmFloat xMax;
		public FsmBool clampY;
		public FsmFloat yMin;
		public FsmFloat yMax;
		[Tooltip("Smooth rotation using Slerp")]
		public FsmBool smoothRotation;
		[HasFloatSlider(1f,15)]
		public FsmFloat smoothFactor;
		[Tooltip("will offset our camera to keep it facing correctly from it primary set rotation, concern our Y angle")]
		public FsmFloat rotationOffset;
		
		Transform cachedTransform = null;
		float x = 0f;
		float y = 0f;
		float usedX = 0f;
		float usedY = 0f;
		
		public override void Reset ()
		{
			gameObject = null;
			inputRotationX = new FsmFloat {UseVariable = true};
			inputRotationY = new FsmFloat {UseVariable = true};
			sensitivityX = 5f;
			sensitivityY = 5f;
			clampX = false;
			xMin = -40f;
			xMax = 40f;
			clampY = true;
			yMin = -80f;
			yMax = 80f;
			smoothRotation = false;
			smoothFactor = 1f;
			rotationOffset = 0f;
		}
		
		public override void OnEnter ()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
				Finish();
			else
				cachedTransform = go.transform;
		}
		
		public override void OnUpdate ()
		{
			GetInput();
		}
		
		public override void OnLateUpdate ()
		{
			DoOrbit();
		}
		
		void GetInput()
		{
			x += inputRotationX.Value * sensitivityX.Value;
			y -= inputRotationY.Value * sensitivityY.Value;
			
			// clamp our input
			if(clampX.Value)
				x = ClampAngle(x, xMin.Value, xMax.Value);
			
			if(clampY.Value)
				y = ClampAngle(y, yMin.Value, yMax.Value);
		}
		
		void DoOrbit()
		{
			// determine if we let value inchanged for x and y
			if(inputRotationX.IsNone)
				usedX = cachedTransform.rotation.eulerAngles.x;
			else
				usedX = x;
			
			if(inputRotationY.IsNone)
				usedY = cachedTransform.rotation.eulerAngles.y;
			else
				usedY = y;
			
			// do our rotation stuff
			if(!smoothRotation.Value)
				cachedTransform.rotation = Quaternion.Euler(usedX, usedY + rotationOffset.Value, 0f);
			else
			{
				var lastRotation = cachedTransform.rotation;
				var rotation = Quaternion.Euler(usedX, usedY + rotationOffset.Value, 0f);
				cachedTransform.rotation = Quaternion.Slerp(lastRotation, rotation, Time.deltaTime * smoothFactor.Value);
			}
		}
		
		static float ClampAngle(float angle, float min, float max)
		{
			if(angle < -360f)
				angle += 360f;
			if(angle > 360f)
				angle -= 360f;
		
			return Mathf.Clamp(angle, min, max);
		}
	}
}
