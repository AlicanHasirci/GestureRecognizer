using UnityEngine;

namespace PDollarGestureRecognizer.Editor {
    public static class PDollarContextMenu {
        private const float Padding = 5f;
        private static Vector2 _size;
        private static Rect _rect;
        private static bool _visible;
        
        private static Vector2 Position {
            set {
                _rect = new Rect(value, _size);
            }
        }
        
        public static void Show(Vector2 position) {
            Position = position;
            _visible = true;
            PDollarEditorWindow.Repaint();
        }

        public static void Hide() {
            _visible = false;
            PDollarEditorWindow.Repaint();
        }

        public static void Draw() {
            if (!_visible) return;
            var items = PDollarInputSystem.MenuItems;
            var buttonSize = new Vector2(140, 25);
            _size = new Vector2(
                Padding + buttonSize.x + Padding,
                (items.Count * (buttonSize.y + Padding)) - Padding);
            GUI.BeginGroup(_rect);
            for (var i = 0; i < items.Count; i++) {
                var item = items[i];
                var rect = new Rect(new Vector2(Padding, (Padding + buttonSize.y) * i), buttonSize);
                if (GUI.Button(rect, item.Key.Title)) {
                    item.Value.DynamicInvoke();
                    Hide();
                }
            }
            GUI.EndGroup();
        }

        [PDollarInputSystem.EventHandlerAttribute(EventType.MouseDown, 0)]
        public static void MenuEvents(Event @event) {
            if ((@event.button == 1)) {
                Show(@event.mousePosition);
            }
            if (@event.button == 0 && !_rect.Contains(@event.mousePosition)) {
                Hide();
            }
        }
    }

}