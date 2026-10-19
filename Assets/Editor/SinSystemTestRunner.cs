using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

public static class SinSystemTestRunner
{
    private const string AssemblyName = "DantesInferno.SinSystem.Tests";
    private static TestRunnerApi activeRunner;

    [MenuItem("Tools/Dante's Inferno/Run Sin System Tests")]
    public static void Run()
    {
        if (activeRunner != null)
        {
            Debug.LogWarning("SIN_TESTS_ALREADY_RUNNING");
            return;
        }

        activeRunner = ScriptableObject.CreateInstance<TestRunnerApi>();
        activeRunner.RegisterCallbacks(new Callbacks());
        activeRunner.Execute(new ExecutionSettings(new Filter
        {
            assemblyNames = new[] { AssemblyName },
            testMode = TestMode.EditMode
        }));
    }

    private sealed class Callbacks : ICallbacks
    {
        public void RunStarted(ITestAdaptor testsToRun)
        {
            Debug.Log($"SIN_TESTS_STARTED count={testsToRun.TestCaseCount}");
        }

        public void RunFinished(ITestResultAdaptor result)
        {
            Debug.Log($"SIN_TESTS_FINISHED status={result.TestStatus} pass={result.PassCount} fail={result.FailCount} skip={result.SkipCount}");
            Object.DestroyImmediate(activeRunner);
            activeRunner = null;
        }

        public void TestStarted(ITestAdaptor test)
        {
        }

        public void TestFinished(ITestResultAdaptor result)
        {
            if (result.TestStatus == TestStatus.Failed)
                Debug.LogError($"SIN_TEST_FAILED name={result.FullName} message={result.Message} stack={result.StackTrace}");
        }
    }
}
