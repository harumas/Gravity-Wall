// <auto-generated>
// This code was generated by AnimatorWrapperGenerator.
// </auto-generated>

using System;
using UnityEngine;

namespace PropertyGenerator.Generated
{
	[Serializable]
	public class PlayerControllerWrapper
	{
		[SerializeField] private Animator target;
		
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
		
		public bool IsDeath
		{
			get => target.GetBool(569220492);
			set => target.SetBool(569220492, value);
		}
		
		public float LandingSpeed
		{
			get => target.GetFloat(408814760);
			set => target.SetFloat(408814760, value);
		}
		
		public int FallIndex
		{
			get => target.GetInteger(-479160146);
			set => target.SetInteger(-479160146, value);
		}
	}
}