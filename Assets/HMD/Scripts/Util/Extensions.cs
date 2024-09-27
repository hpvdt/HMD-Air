using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HMD.Scripts.Util
{
    public static class Extensions
    {
        // for UnityEvent
        public static void Rebind(this UnityEvent self, UnityAction call)
        {
            self.RemoveAllListeners();
            self.AddListener(call);
        }

        public static void Rebind<T>(this UnityEvent<T> self, UnityAction<T> call)
        {
            self.RemoveAllListeners();
            self.AddListener(call);
        }

        public static EventTrigger.TriggerEvent OnEvent(this Component self, EventTriggerType id)
        {
            var events = self.GetComponent<EventTrigger>();
            var entry = new EventTrigger.Entry
            {
                eventID = id
            };
            events.triggers.Add(entry);

            var result = entry.callback;
            return result;
            // entry.callback.AddListener(action);
        }

        public static Quaternion DropRoll(this Quaternion self)
        {
            var euler = self.eulerAngles;
            euler.z = 0;

            return Quaternion.Euler(euler);
        }
    }
}