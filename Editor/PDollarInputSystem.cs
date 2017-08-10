using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PDollarGestureRecognizer.Editor
{
	public static class PDollarInputSystem
	{
		private static List<KeyValuePair<EventHandlerAttribute, Delegate>> _eventHandlers;
		private static List<KeyValuePair<MenuItemAttribute, Delegate>> _menuItemHandlers;

		public static List<KeyValuePair<MenuItemAttribute, Delegate>> MenuItems
		{
			get { return _menuItemHandlers; }
		}
		
		public static void SetupInput () 
		{
			_eventHandlers = new List<KeyValuePair<EventHandlerAttribute, Delegate>> ();
			_menuItemHandlers = new List<KeyValuePair<MenuItemAttribute, Delegate>> ();
			IEnumerable<Assembly> scriptAssemblies = AppDomain.CurrentDomain.GetAssemblies ().Where ((Assembly assembly) => assembly.FullName.Contains ("Assembly"));
			foreach (Assembly assembly in scriptAssemblies) 
			{
				foreach (Type type in assembly.GetTypes ()) 
				{
					foreach (MethodInfo method in type.GetMethods (BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)) 
					{
						Delegate actionDelegate = null;
						foreach (object attr in method.GetCustomAttributes (true))
						{
							Type attrType = attr.GetType ();
							if (attrType == typeof(EventHandlerAttribute))
							{
								if (EventHandlerAttribute.AssureValidity(method, attr as EventHandlerAttribute))
								{
									if (actionDelegate == null) actionDelegate = Delegate.CreateDelegate(typeof(Action<Event>), method);
									_eventHandlers.Add(
										new KeyValuePair<EventHandlerAttribute, Delegate>(attr as EventHandlerAttribute, actionDelegate));
								}
							}
							if (attrType == typeof(MenuItemAttribute))
							{
								if (actionDelegate == null) actionDelegate = Delegate.CreateDelegate(typeof(Action), method);
								_menuItemHandlers.Add(
									new KeyValuePair<MenuItemAttribute, Delegate>(attr as MenuItemAttribute, actionDelegate));
							}
						}
					}
				}
			}
			_eventHandlers.Sort ((handlerA, handlerB) => handlerA.Key.Priority.CompareTo (handlerB.Key.Priority));
		}
		
		public static void CallEventHandlers () 
		{
			var current = Event.current;
			foreach (KeyValuePair<EventHandlerAttribute, Delegate> eventHandler in _eventHandlers)
			{
				if ((eventHandler.Key.HandledEvent == null || eventHandler.Key.HandledEvent == current.type) &&
				    (eventHandler.Key.Priority < 100))
				{
					eventHandler.Value.DynamicInvoke (current);
					if (current.type == EventType.Used) return;
					
				}
			}
		}
		
		[AttributeUsage (AttributeTargets.Method, AllowMultiple = true)]
		public class EventHandlerAttribute : Attribute 
		{
			public EventType? HandledEvent { get; private set; }
			public int Priority { get; private set; }

			public EventHandlerAttribute(EventType eventType, int priorityValue)
			{
				HandledEvent = eventType;
				Priority = priorityValue;
			}

			public EventHandlerAttribute(int priorityValue)
			{
				HandledEvent = null;
				Priority = priorityValue;
			}

			public EventHandlerAttribute (EventType eventType) 
			{
				HandledEvent = eventType;
				Priority = 50;
			}

			public EventHandlerAttribute () 
			{
				HandledEvent = null;
			}

			internal static bool AssureValidity (MethodInfo method, EventHandlerAttribute attr) 
			{
				if (!method.IsGenericMethod && !method.IsGenericMethodDefinition && method.ReturnType == typeof(void))
				{
					var methodParams = method.GetParameters ();
					if (methodParams.Length == 1 && methodParams[0].ParameterType == typeof(Event))
						return true;
					else
						Debug.LogWarning ("Method " + method.Name + " has incorrect signature for EventHandlerAttribute!");
				}
				return false;
			}
		}

		[AttributeUsage (AttributeTargets.Method, AllowMultiple = true)]
		public class MenuItemAttribute : Attribute
		{
			public string Title;

			public MenuItemAttribute(string title)
			{
				Title = title;
			}
		}
	}
}
