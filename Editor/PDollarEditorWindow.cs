using System.Resources;
using UnityEditor;
using UnityEngine;

namespace PDollarGestureRecognizer.Editor
{
    public class PDollarEditorWindow : EditorWindow
    {
        private static PDollarEditorWindow _editor;
        public static PDollarEditorWindow Editor {
            get
            {
                if (_editor == null) Init();
                return _editor;
            }
        }

        public static Rect WindowRect
        {
            get { return Editor.position; }
        }

        public static Vector3 Origin
        {
            get { return new Vector3(WindowRect.width * .5f, WindowRect.height * .5f); }
        }

        [MenuItem("Window/Gesture Creator %g")]
        static void Init()
        {
            _editor = GetWindow<PDollarEditorWindow>(title: "Gesture Creator", focus: true);
            _editor.wantsMouseMove = true;
            _editor.Show();
        }

        void OnGUI()
        {
            DrawGrid();
            GestureDrawer.Draw();
            PDollarInputSystem.CallEventHandlers();
            
            PDollarContextMenu.Draw();
        }

        void OnEnable()
        {
            PDollarInputSystem.SetupInput();
        }

        private void OnLostFocus()
        {
            PDollarContextMenu.Hide();
        }

        public new static void Repaint()
        {
            ((EditorWindow) Editor).Repaint();
        }

        private Texture2D _background;
        private void DrawGrid()
        {
            var size = WindowRect.size;
            GridDrawer.Draw(new Rect(Vector2.zero, size));
            Handles.color = Color.gray;
            Handles.DrawLines(new []
            {
                Origin - new Vector3(size.x * .5f, 0, 0), Origin + new Vector3(size.x * .5f, 0, 0),
                Origin - new Vector3(0, size.y * .5f, 0), Origin + new Vector3(0, size.y * .5f, 0)
            });
        }
    }
}
