using UnityEngine;

namespace Eggshell.Unity.Inputs
{
	public class Axis : Bind
	{
		public string Sampler { get; }
		public bool Raw { get; }

		public Axis( string name, string axis, bool raw = true ) : base( name )
		{
			Sampler = axis;
			Raw = raw;
		}

		public override void Sample()
		{
			Value = Raw ? Input.GetAxisRaw( Sampler ) : Input.GetAxis( Sampler );
			Held = Value > 0;
		}

		public override void Clear()
		{
			Value = 0;
		}
	}
}
