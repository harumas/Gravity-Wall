// <auto-generated>
// This code was generated by AnimatorWrapperGenerator.
// </auto-generated>

using UnityEngine;

namespace PropertyGenerator.Generated
{
	public readonly struct PlayerWrapper
	{
		private readonly Animator target;
		
		public PlayerWrapper(Animator target)
		{
			this.target = target;
		}
		
		public bool Walk
		{
			get => target.GetBool(765711723);
			set => target.SetBool(765711723, value);
		}
		
		public void SetRotatingTrigger()
		{
			target.SetTrigger(-1328712894);
		}
		
		public void SetJumpTrigger()
		{
			target.SetTrigger(125937960);
		}
	}
}