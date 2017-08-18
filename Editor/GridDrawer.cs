using UnityEditor;
using UnityEngine;

namespace PDollarGestureRecognizer.Editor {
	public static class GridDrawer {
		private static Texture2D _background;

		public static void Draw(Rect rect) {
			if (_background == null) {
				_background = Resources.Load<Texture2D>("Textures/background");
			}
			
			Rect uvDrawRect = new Rect (
				-((rect.width * .5f) %_background.width) / _background.width,
				-((rect.height * .5f) %_background.height) / _background.height,
				rect.width / _background.width,
				rect.height / _background.height);
			
			GUI.DrawTextureWithTexCoords (rect, _background, uvDrawRect);
		}
	}
}
