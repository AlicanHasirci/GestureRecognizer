using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;

namespace PDollarGestureRecognizer.Editor
{
	[CustomEditor(typeof(Gesture))]
	public class GestureInspector : UnityEditor.Editor
	{
		public override bool HasPreviewGUI() { return true; }

		private Gesture _gesture;
		private Texture2D _background;

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			_gesture = (Gesture) target;
			if (Event.current.type == EventType.Repaint)
			{
				GridDrawer.Draw(r);
				Handles.color = Color.cyan;
				DrawGesture(r);
			}
		}

		private void DrawGesture(Rect r)
		{
			var scale = r.width / r.height > 0 ? r.height * .75f : r.width * .75f;
			var translate = r.position + (r.size * .5f);
			var transform = Matrix4x4.Translate(translate) * Matrix4x4.Scale(Vector3.one * scale);
			for (int i = 0; i < _gesture.Points.Length - 1; i++)
			{
				var curr = _gesture.Points[i];
				var next = _gesture.Points[i + 1];
				if (curr.Id != next.Id) continue;
				Handles.DrawLine(
					transform.MultiplyPoint3x4(curr.Position3D),
					transform.MultiplyPoint3x4(next.Position3D));
			}
		}
	}
}