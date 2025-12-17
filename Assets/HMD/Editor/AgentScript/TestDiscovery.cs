using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HMD.Editor.AgentScript
{
    public static class TestDiscovery
    {
        [MenuItem("Tools/Agent Scripts/Discover All Tests")]
        public static void DiscoverAllTests()
        {
            Debug.Log("=== DISCOVERING ALL TESTS ===");

            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            int totalTests = 0;

            foreach (var assembly in assemblies)
            {
                if (assembly.GetName().Name.Contains("Test"))
                {
                    Debug.Log($"Found test assembly: {assembly.GetName().Name}");

                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        var methods =
                            type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                        foreach (var method in methods)
                        {
                            var attributes = method.GetCustomAttributes(true);
                            bool isTest = attributes.Any(attr =>
                                attr.GetType().Name.Contains("Test") ||
                                attr.GetType().Name.Contains("NUnit"));

                            if (isTest)
                            {
                                Debug.Log($"Found test: {type.FullName}.{method.Name}");
                                totalTests++;
                            }
                        }
                    }
                }
            }

            Debug.Log($"=== TOTAL TESTS DISCOVERED: {totalTests} ===");
        }
    }
}