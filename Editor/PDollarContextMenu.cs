using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace PDollarGestureRecognizer.Editor
{
    public static class PDollarContextMenu
    {
        private static Vector2 Size;
        private static Rect _rect;
        private static bool _visible = false;
        
        public static void Show(Vector2 position)
        {
            Position = position;
            _visible = true;
            PDollarEditorWindow.Repaint();
        }

        public static void Hide()
        {
            _visible = false;
            PDollarEditorWindow.Repaint();
        }

        private static Vector2 Position
        {
            set
            {
                _rect = new Rect(value, Size);
            }
        }

        public static void Draw()
        {
            if (!_visible) return;
            var items = PDollarInputSystem.MenuItems;
            var buttonSize = new Vector2(140, 25);
            var padding = 5f;
            Size = new Vector2(
                padding + buttonSize.x + padding,
                (items.Count * (buttonSize.y + padding)) - padding);
            GUI.BeginGroup(_rect);
//            if (GUI.Button(new Rect(new Vector2(5, 0), buttonSize), "New"))
//            {
//                Hide();
//                GestureDrawer.New();
//            }
//            if (GUI.Button(new Rect(new Vector2(5, 30), buttonSize), "Save"))
//            {
//                Hide();
//                GestureDrawer.Save();
//            }
//            if (GUI.Button(new Rect(new Vector2(5, 60), buttonSize), "Load"))
//            {
//                Hide();
//                GestureDrawer.Load();
//            }
//            if (GUI.Button(new Rect(new Vector2(5, 90), buttonSize), "Classify"))
//            {
//                Hide();
//                GestureDrawer.Classify();
//            }
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var rect = new Rect(new Vector2(padding, (padding + buttonSize.y) * i), buttonSize);
                if (GUI.Button(rect, item.Key.Title))
                {
                    item.Value.DynamicInvoke();
                    Hide();
                }
            }
            GUI.EndGroup();
        }

        [PDollarInputSystem.EventHandlerAttribute(EventType.MouseDown, 0)]
        public static void MenuEvents(Event @event)
        {
            if ((@event.button == 1))
            {
                Show(@event.mousePosition);
            }
            if (@event.button == 0 && !_rect.Contains(@event.mousePosition))
            {
                Hide();
            }
        }
    }

}