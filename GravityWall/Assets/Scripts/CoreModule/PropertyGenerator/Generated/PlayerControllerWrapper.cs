// <auto-generated>
// This code was generated by AnimatorWrapperGenerator.
// </auto-generated>

using UnityEngine;

namespace PropertyGenerator.Generated
{
	public readonly struct PlayerControllerWrapper
	{
		private readonly Animator target;
		
		public PlayerControllerWrapper(Animator target)
		{
			this.target = target;
		}
		
		public bool IsRotating
		{
			get => target.GetBool(-1645996249);
			set => target.SetBool(-1645996249, value);
		}
		
		public bool IsJumping
		{
			get => target.GetBool(1749078233);
			set => target.SetBool(1749078233, value);
		}
		
		public float Speed
		{
			get => target.GetFloat(-823668238);
			set => target.SetFloat(-823668238, value);
		}
	}
}