using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Chimera
{
	public enum ChimeraEvent
	{
		[Chimera.StringValue("Default")]Default,
		[Chimera.StringValue("CalculateComplete")]CalculateComplete,
		[Chimera.StringValue("DrawComplete")]DrawComplete,
		[Chimera.StringValue("HoverOnTile")]HoverOnTile,
		[Chimera.StringValue("OnClick")]OnMouseClick,
	}

	public struct ChimeraEventResponce
	{
		public ChimeraEvent Type {get;set;}
		public object Data {get;set;}
		public EventDelegat Callback {get;set;}
		public Vector3 mousePosition{get;set;}
	}

	public delegate void EventDelegat(Chimera.ChimeraEventResponce chEvent);

	public class ChimeraEventDispather
	{
		static private ChimeraEventDispather instance;


		private Dictionary<string, List<ChimeraEventResponce>> listeners = new Dictionary<string, List<ChimeraEventResponce>>();


		public static ChimeraEventDispather Instance
		{
			get{
				if (instance == null)
				{
					instance = new ChimeraEventDispather();
				}

				return instance;
			}
		}

		public void DispatchEvent(ChimeraEvent type, Vector3 mousePosition)
		{
			List<ChimeraEventResponce> listener = null;
			if (listeners.ContainsKey(StringEnum.GetStringValue(type)))
			{
				listeners.TryGetValue(StringEnum.GetStringValue(type), out listener);
				
				if (listener != null)
				{
					for (int i = 0; i < listener.Count; i++)
					{
						ChimeraEventResponce responce = listener[i];
						responce.mousePosition = mousePosition;

						if(responce.Callback != null)
						{
							responce.Callback(responce);
						}
					}
				}
			}
		}

		public void DispatchEvent(ChimeraEvent type)
		{
			List<ChimeraEventResponce> listener = null;
			if (listeners.ContainsKey(StringEnum.GetStringValue(type)))
			{
				listeners.TryGetValue(StringEnum.GetStringValue(type), out listener);

				if (listener != null)
				{
					for (int i = 0; i < listener.Count; i++)
					{
						ChimeraEventResponce response = listener[i];

						if(response.Callback != null)
						{
							response.Callback(response);
						}
					}
				}
			}
		}

		public void AddEventListener(ChimeraEvent type, object data, EventDelegat callback)
		{
			ChimeraEventResponce result = new ChimeraEventResponce();
			List<ChimeraEventResponce> listener = null;

			string key = StringEnum.GetStringValue(type);

			if (!listeners.ContainsKey(key))
			{
				listeners.Add(StringEnum.GetStringValue(type), null);
			}

			listeners.TryGetValue(StringEnum.GetStringValue(type), out listener);

			if (listener == null)
			{
				listener = new List<ChimeraEventResponce>();
			}

			if (!HasEventListener(type, callback))
			{
				result.Type = type;
				result.Data = data;
				result.Callback = callback;
				listener.Add(result);
				listeners[StringEnum.GetStringValue(type)] = listener;
			}
			
		}

		public bool HasEventListener(ChimeraEvent type, EventDelegat callback)
		{
			bool result = false;
			List<ChimeraEventResponce> listener = null;

			if (listeners.ContainsKey(StringEnum.GetStringValue(type)))
			{
				listeners.TryGetValue(StringEnum.GetStringValue(type), out listener);

				if (listener != null)
				{
					for (int i = 0; i < listener.Count; i++)
					{
						//				System.Runtime.Serialization.SerializationInfo info;
						//				System.Runtime.Serialization.StreamingContext contex;
						//				callback.GetObjectData(info,contex);
						
						ChimeraEventResponce responce = listener[i];
						if (responce.Callback == callback)
						{
							result = true;
							break;
						}
					}
				}
			}
			return result;
		}

		public void RemoveEventListener(ChimeraEvent type, EventDelegat callback)
		{
			List<ChimeraEventResponce> listener = null;
			if (listeners.ContainsKey(StringEnum.GetStringValue(type)))
			{
				listeners.TryGetValue(StringEnum.GetStringValue(type), out listener);

				for (int i = 0; i < listener.Count; i++)
				{
					ChimeraEventResponce responce = listener[i];
					if (responce.Callback == callback)
					{
						listener.Remove(responce);
						listeners[StringEnum.GetStringValue(type)] = listener;
						break;
					}
				}
			}
		}
	}
}