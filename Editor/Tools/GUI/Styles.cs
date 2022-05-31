using UnityEditor;
using UnityEngine;

namespace Eggshell.Unity.Tools
{
	public static class Styles
	{
		// GUI Elements

		public static void Line( float opacity = 0.4f, params GUILayoutOption[] options )
		{
			GUI.color = GUI.color.WithAlpha( opacity );
			GUILayout.Label( "", Underline, options );
			GUI.color = GUI.color.WithAlpha( 1 );
		}
		
		public static void Footer( string title, bool flex = true )
		{
			if ( flex )
				GUILayout.FlexibleSpace();

			Line();
			GUILayout.Label( title, EditorStyles.centeredGreyMiniLabel );
		}

		// GUI Style

		public static GUIStyle Panel { get; } = new( GUI.skin.window )
		{
			margin = new( 8, 8, 8, 8 ),
			padding = new( 8, 8, 8, 8 )
		};

		public static GUIStyle Board { get; } = new( Panel )
		{
			margin = new( 2, 2, 2, 2 )
		};

		public static GUIStyle Button { get; } = new( Panel )
		{
			contentOffset = Vector2.zero,
			margin = new( 0, 0, 0, 0 ),
			padding = new( 8, 8, 8, 8 ),
			alignment = TextAnchor.MiddleCenter,
			imagePosition = ImagePosition.ImageAbove
		};

		public static GUIStyle Title { get; } = new( GUI.skin.label )
		{
			fontSize = 24,
			richText = true,
			imagePosition = ImagePosition.TextOnly
		};

		public static GUIStyle Header { get; } = new( Title )
		{
			fontSize = 18,
			richText = true,
			imagePosition = ImagePosition.TextOnly
		};

		public static GUIStyle Description { get; } = new( GUI.skin.label )
		{
			fontSize = 12,
			richText = true,
			imagePosition = ImagePosition.TextOnly,
			wordWrap = true
		};

		public static GUIStyle Blurb { get; } = new( GUI.skin.label )
		{
			fontSize = 9,
			richText = true,
			imagePosition = ImagePosition.TextOnly,
			wordWrap = true
		};

		public static GUIStyle Underline { get; } = new( GUI.skin.horizontalSlider )
		{
			margin = new( 8, 8, 0, 16 )
		};
	}
}
