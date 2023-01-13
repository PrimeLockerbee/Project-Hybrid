using System.Collections.Generic;

namespace MarcoHelpers
{
    public enum EventName
    {
        TILE_CLICKED = 0,
        DAM_CHANGED = 1,
        FUEL_EMPTY = 2
    }

    public delegate void EventCallback(object _value);

    public static class EventSystem
    {
        private static Dictionary<EventName, List<EventCallback>> eventRegister = new Dictionary<EventName, List<EventCallback>>();

        public static void Subscribe(EventName _evt, EventCallback _func)
        {
            if (!eventRegister.ContainsKey(_evt))
            {
                eventRegister[_evt] = new List<EventCallback>();
            }

            eventRegister[_evt].Add(_func);
        }

        public static void Unsubscribe(EventName _evt, EventCallback _func)
        {
            if (eventRegister.ContainsKey(_evt))
            {
                eventRegister[_evt].Remove(_func);
            }
        }

        public static void RaiseEvent(EventName _evt, object _value = null)
        {
            if (eventRegister.ContainsKey(_evt))
            {
                foreach (EventCallback e in eventRegister[_evt])
                {
                    e.Invoke(_value);
                }
            }
        }
    }
} 

