using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace HMD.Editor.AgentScript
{
    public class TestRunnerHelper : ICallbacks
    {
        private static List<string> _testResults = new List<string>();
        private static TestRunnerApi _testRunnerApi;

        [MenuItem("Tools/Agent Scripts/Run All PlayMode Tests")]
        public static void RunAllPlayModeTests()
        {
            Debug.Log("Starting PlayMode tests...");

            _testResults.Clear();
            _testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();

            var helper = new TestRunnerHelper();
            _testRunnerApi.RegisterCallbacks(helper);

            var filter = new Filter()
            {
                testMode = TestMode.PlayMode
            };

            _testRunnerApi.Execute(new ExecutionSettings(filter));
        }

        [MenuItem("Tools/Agent Scripts/Run All EditMode Tests")]
        public static void RunAllEditModeTests()
        {
            Debug.Log("Starting EditMode tests...");

            _testResults.Clear();
            _testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();

            var helper = new TestRunnerHelper();
            _testRunnerApi.RegisterCallbacks(helper);

            var filter = new Filter()
            {
                testMode = TestMode.EditMode
            };

            _testRunnerApi.Execute(new ExecutionSettings(filter));
        }

        [MenuItem("Tools/Agent Scripts/List Test Results")]
        public static void ListTestResults()
        {
            Debug.Log("=== TEST RESULTS ===");
            if (_testResults.Count == 0)
            {
                Debug.Log("No test results available. Run tests first.");
            }
            else
            {
                foreach (var result in _testResults)
                {
                    Debug.Log(result);
                }
            }

            Debug.Log("=== END TEST RESULTS ===");
        }

        // ICallbacks implementation
        public void RunStarted(ITestAdaptor testsToRun)
        {
            Debug.Log($"Test run started with {CountTests(testsToRun)} tests");
        }

        public void RunFinished(ITestResultAdaptor result)
        {
            Debug.Log(
                $"Test run finished. Passed: {result.PassCount}, Failed: {result.FailCount}, Skipped: {result.SkipCount}");
            ListAllTestResults(result);
        }

        public void TestStarted(ITestAdaptor test)
        {
            Debug.Log($"Starting test: {test.FullName}");
        }

        public void TestFinished(ITestResultAdaptor result)
        {
            string status = result.TestStatus.ToString();
            string resultText = $"Test: {result.Test.FullName} - Status: {status}";

            if (result.TestStatus == TestStatus.Passed)
            {
                resultText += " ✓ PASSED";
            }
            else if (result.TestStatus == TestStatus.Failed)
            {
                resultText += " ✗ FAILED";
                if (!string.IsNullOrEmpty(result.Message))
                    resultText += $" - {result.Message}";
            }

            _testResults.Add(resultText);
            Debug.Log(resultText);
        }

        private static int CountTests(ITestAdaptor test)
        {
            int count = 0;
            if (test.HasChildren)
            {
                foreach (var child in test.Children)
                {
                    count += CountTests(child);
                }
            }
            else
            {
                count = 1;
            }

            return count;
        }

        private static void ListAllTestResults(ITestResultAdaptor result)
        {
            if (result.HasChildren)
            {
                foreach (var child in result.Children)
                {
                    ListAllTestResults(child);
                }
            }
            else
            {
                string status = result.TestStatus.ToString();
                string resultText = $"Final Result - {result.Test.FullName}: {status}";
                if (result.TestStatus == TestStatus.Passed)
                    resultText += " ✓";
                else if (result.TestStatus == TestStatus.Failed)
                    resultText += " ✗";

                Debug.Log(resultText);
            }
        }
    }
}