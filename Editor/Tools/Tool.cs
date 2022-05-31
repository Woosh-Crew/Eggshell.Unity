using UnityEditor;
using UnityEngine;

namespace Eggshell.Unity.Tools
{
	/// <summary>
	/// A Tool is a Unity Editor Window, with some nice eggshell things
	/// attached to it, Such as setting the title content from the ClassInfo
	/// meta data (Title and Help) as well as using Attributes for stylesheets.
	/// </summary>
	public abstract class Tool : IObject
	{
		public Library ClassInfo { get; }

		public Tool()
		{
			ClassInfo = Library.Register( this );
		}

		~Tool()
		{
			Library.Unregister( this );
		}

		public virtual void OnGUI() { }

		public class Browser : EditorWindow
		{
			[MenuItem( "Eggshell/Tool Browser" )]
			public static void Open()
			{
				GetWindow<Browser>().Show();
			}

			private void OnEnable()
			{
				titleContent = new GUIContent( "Tools Browser", "Browse all Eggshell Unity Editor tools through this handy window" );
			}

			private void OnGUI()
			{
				using ( new GUILayout.HorizontalScope() )
				{
					using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.Width( 140 ) ) )
					{
						for ( int i = 0; i < 10; i++ )
						{
							GUILayout.Button( "Group Name", Styles.Button );
							GUILayout.Space( 8 );
						}

						Styles.Footer( "Groups" );
					}

					using ( new GUILayout.VerticalScope( Styles.Panel ) )
					{
						GUILayout.Label( "[Group Name]", Styles.Header );
						Styles.Line( 1 );


						using ( new GUILayout.VerticalScope( Styles.Panel ) )
						{
							GUILayout.Button( "im a tool" );
						}

						Styles.Footer( "Tools" );
					}
				}
			}
		}
	}
}
