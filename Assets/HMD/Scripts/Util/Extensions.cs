namespace HMD.Scripts.Util
{
    using UnityEngine.Events;

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
    }
}
