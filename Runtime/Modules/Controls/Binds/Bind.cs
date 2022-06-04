namespace Eggshell.Unity.Inputs
{
	public abstract class Bind
	{
		public string Name { get; }

		public Bind( string name )
		{
			Name = name;
		}

		public abstract void Sample();
		public abstract void Clear();

		// Networking
		public virtual void Write() { }
		public virtual void Read() { }

		public float Value { get; protected set; }
		public bool Pressed { get; protected set; }
		public bool Held { get; protected set; }
		public bool Released { get; protected set; }
	}
}
