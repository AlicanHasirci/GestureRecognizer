using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PDollarGestureRecognizer.Editor {
	public static class GestureDrawer {
		private const int SampleSize = 32;
		private static readonly LinkedList<Point> PointBuffer = new LinkedList<Point>();

		private static int _strokeId;
		private static bool _drawing;
		
		[PDollarInputSystem.MenuItem("New")]
		public static void New() {
			_strokeId = 0;
			PointBuffer.Clear();
		}

		[PDollarInputSystem.MenuItem("Save")]
		public static void Save() {
			var path = EditorUtility.SaveFilePanel("Save Gesture", Application.dataPath, "", "asset");
			path = TextUtil.GetRelativePath(path);
			if (path.Length != 0) {
				var points = PointBuffer.ToArray();
				points = points.Normalize(SampleSize, PointOrigin.TopLeft);
				var gesture = Gesture.Build(TextUtil.GetFilename(path), points);
				AssetDatabase.CreateAsset(gesture, path);
				AssetDatabase.SaveAssets();
			}
		}

		[PDollarInputSystem.MenuItem("Load")]
		public static void Load() {
			var path = EditorUtility.OpenFilePanel("Load Gesture", Application.dataPath,"asset");
			path = TextUtil.GetRelativePath(path);
			var gesture = AssetDatabase.LoadAssetAtPath<Gesture>(path);
			DumpPointsToBuffer(gesture.Points);
			PDollarEditorWindow.Repaint();
		}

		[PDollarInputSystem.MenuItem("Classify")]
		public static void Classify() {
			var points = PointBuffer.ToArray().Normalize(SampleSize, PointOrigin.TopLeft);
			var gestures = Resources.LoadAll<Gesture>("");
			var winner = PointCloudRecognizer.Classify(points, gestures);
			Debug.Log(string.Format("<color=#57A64A>-->Winner is {0}!!!</color>",winner));
		}
		
		[PDollarInputSystem.MenuItem("Build")]
		public static void Build() {
			var normalizedPoints = PointBuffer.ToArray().Normalize(SampleSize, PointOrigin.TopLeft);
			DumpPointsToBuffer(normalizedPoints);
		}

		public static void Draw() {
			Handles.color = Color.green;
			DrawPointBuffer();
		}

		private static void DrawPointBuffer() {
			if (PointBuffer.Count == 0) return;
			var node = PointBuffer.First;
			while (node.Next != null) {
				var curr = node.Value;
				var next = node.Next.Value;
				if (curr.Id == next.Id) Handles.DrawLine(curr.Position3D, next.Position3D);
				Handles.CircleHandleCap(0, curr.Position3D, Quaternion.identity, 5, EventType.Repaint);
				node = node.Next;
			}
			Handles.CircleHandleCap(0, PointBuffer.Last.Value.Position3D, Quaternion.identity, 5, EventType.Repaint);
		}

		private static void DumpPointsToBuffer(Point[] points) {
			PointBuffer.Clear();
			_strokeId = points[points.Length - 1].Id + 1;
			var windowSize = PDollarEditorWindow.WindowRect.size;
			var scale = windowSize.x / windowSize.y > 0 ? windowSize.y * .75f : windowSize.x * .75f;
			var transform = Matrix4x4.Translate(PDollarEditorWindow.Origin) * Matrix4x4.Scale(new Vector3(1, -1, 1) * scale);
			foreach (var point in points) {
				PointBuffer.AddLast(
					new Point() {
						Id = point.Id,
						Position3D = transform.MultiplyPoint3x4(point.Position3D)
				});
			}
		}

		[PDollarInputSystem.EventHandlerAttribute(EventType.MouseDrag, 0)]
		public static void MenuEvents(Event @event) {
			if (@event.button == 0) {
				_drawing = true;
				PointBuffer.AddLast(new Point(@event.mousePosition, _strokeId));
				PDollarEditorWindow.Repaint();
			}
		}
		
		[PDollarInputSystem.EventHandlerAttribute(EventType.MouseUp, 0)]
		public static void MouseUp(Event @event) {
			if (@event.button == 0 && _drawing) {
				_drawing = false;
				_strokeId++;
			}
		}
	}
}
