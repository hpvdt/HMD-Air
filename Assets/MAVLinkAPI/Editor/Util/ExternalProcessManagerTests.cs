using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MAVLinkAPI.Scripts.Util;
using NUnit.Framework;

namespace MAVLinkAPI.Editor.Util
{
    [TestFixture]
    public class ExternalProcessManagerTests
    {
        [Test]
        public void StartAndMonitor_ProcessCompletes_ReturnsTrue()
        {
            using var manager = new ExternalProcessManager("cmd.exe", "/C ping localhost -n 2");
            var task = Task.Run(() => manager.StartAndMonitorAsync());
            var result = task.Result;
            Assert.IsTrue(result);
        }

        [Test]
        public void StartAndMonitor_ProcessNotResponding_TerminatesProcess()
        {
            using var manager = new ExternalProcessManager("cmd.exe", "/C timeout /t 15");
            var stopwatch = Stopwatch.StartNew();
            var task = Task.Run(() => manager.StartAndMonitorAsync());
            task.Wait();
            stopwatch.Stop();

            Assert.Less(stopwatch.Elapsed.TotalSeconds, 12, "Process should be terminated after about 10 seconds");
        }

        [Test]
        public void Dispose_ProcessRunning_TerminatesProcess()
        {
            int id;
            using (var manager = new ExternalProcessManager("cmd.exe", "/C timeout /t 30"))
            {
                var task = Task.Run(() => manager.StartAndMonitorAsync());
                Thread.Sleep(1000); // Give some time for the process to start
                // process = Process.GetProcessesByName("cmd")[0];
                id = manager._process.Id;
                var p1 = Process.GetProcessById(id);

                p1.Refresh();
                // Assert.IsTrue(process.Responding);
            }

            Thread.Sleep(1000); // Give a short time for the process to be terminated
            // Assert.Throws<InvalidOperationException>(() => process.Refresh());
            var p2 = Process.GetProcessById(id);
            EnsureProcessExited(p2, -2);
        }

        public static void EnsureProcessExited(Process process, int expectedExitCode = 0)
        {
            if (!process.WaitForExit(0))
                throw new TimeoutException("Process did not exit within 0 seconds.");

            if (process.ExitCode != expectedExitCode)
                throw new AssertionException(
                    $"Process exited with code {process.ExitCode}\n" +
                    $"{process.StartInfo.FileName} {process.StartInfo.Arguments}");
        }
    }
}