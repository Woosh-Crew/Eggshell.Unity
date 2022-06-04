using System.Collections;
using System.Collections.Generic;

namespace Eggshell.Unity.Inputs
{
	public class Scheme : IEnumerable<Bind>, IObject
	{
		public Library ClassInfo { get; }

		public Scheme()
		{
			ClassInfo = Library.Register( this );
		}

		~Scheme()
		{
			Library.Unregister( this );
		}

		private readonly SortedList<int, Bind> _binds = new();

		public Bind Get( string key )
		{
			return _binds.TryGetValue( key.Hash(), out var value ) ? value : null;
		}

		public T Get<T>( string key ) where T : Bind
		{
			return (T)Get( key );
		}

		public void Add( Bind bind )
		{
			var key = bind.Name.Hash();

			if ( _binds.ContainsKey( key ) )
			{
				Terminal.Log.Warning( $"Replacing Binding {key}" );
				_binds[key] = bind;
				return;
			}

			_binds.Add( key, bind );
		}

		// Enumerator

		public IEnumerator<Bind> GetEnumerator()
		{
			return _binds.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
