namespace HMD.Scripts.Util
{
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Unity.VisualScripting;
    using UnityEngine;
    
    public static class GlobalFinder
    {
        // TODO: is it already implemented by Unity?
        // private static readonly Dictionary<string, GameObject> Cache = new Dictionary<string, GameObject>();
        //
        // public static void Register(string name, GameObject obj)
        // {
        //     Cache[name] = obj;
        // }

        public class Found
        {
            [NotNull]
            public readonly string Query;
            public readonly GameObject[] All;
            
            public Found(string query, GameObject[] all)
            {
                Query = query;
                All = all;
            }
            
            public  GameObject First()
            {
                if (All.Length == 0)
                    throw new System.Exception("No GameObject with name " + Query + " found in scene.");
            
                return All.First();
            }
            public  GameObject Only()
            {
                if (All.Length > 1)
                    throw new System.Exception($"{All.Length} GameObjects with name " + Query + " found in scene.");

                return First();
            }
        }
        
        public static Found FirstByPath_active(string path)
        {
            var found = GameObject.Find(path);
            var array = (new[] { found }).NotNull().ToArray();
            
            return new Found(path, array);
        }

        public static Found Find(string path)
        {
            //alias
            return FirstByPath_active(path);
        }
        
        // will find both active & inactive objects, unless otherwise specified
        public static Found ByName(string name)
        {
            var found = new List<GameObject>();
            
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var entry in all)
                if (entry.name == name)
                    found.Add(entry);
            
            return new Found(name, found.ToArray());
        }

        public static Found ByName(this GameObject self, string name)
        {
            var found = new List<GameObject>();
            
            var all = self.GetComponentsInChildren<Transform>(true)
                .Select(t => t.gameObject)
                .Distinct()
                .Where(go => go != self)
                .ToArray();
            
            foreach (var entry in all)
            {
                if (entry.name == name)
                    found.Add(entry);
            }
            
            return new Found(name, found.ToArray());
        }
    }
}
