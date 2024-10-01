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
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
        private (string, string) ExeInShell(string cmd)
        {
            return ("cmd.exe", $"/C {cmd}");
        }

        private string WaitCmd(int seconds)
        {
            return
                $"timeout /t {seconds}";
        }

        private string PingCmd => "ping localhost -n 1";
#else
        private (string, string) ExeInShell(string cmd)
        {
            return ("bash", $"-c '{cmd}'");
        }

        private string WaitCmd(int seconds)
        {
            return
                $"for i in {{{seconds}..1}}; do echo -ne \"\\r$i seconds left\"; sleep 1; done; echo -e \"\\nTime'\"'\"'s up!\"";
        }

        private string PingCmd => "ping localhost -c 1";
#endif


        [Test]
        public void Completed()
        {
            var sections = ExeInShell(PingCmd);
            using var manager = new ExternalProcessManager(sections.Item1, sections.Item2);
            var task = Task.Run(() => manager.StartAndMonitorAsync());
            var result = task.Result;
            Assert.IsTrue(result);
            EnsureProcessExited(manager.Process);
        }

        [Test]
        public void TerminatedForNotResponding()
        {
            var sections = ExeInShell(WaitCmd(15));
            using var manager = new ExternalProcessManager(sections.Item1, sections.Item2);
            var stopwatch = Stopwatch.StartNew();
            var task = Task.Run(() => manager.StartAndMonitorAsync());
            task.Wait();
            stopwatch.Stop();

            Assert.Less(stopwatch.Elapsed.TotalSeconds, 12, "Process should be terminated after about 10 seconds");

            Thread.Sleep(1000); // Give a short time for the process to be terminated
            EnsureProcessExited(manager.Process, false);
        }

        [Test]
        public void Disposed()
        {
            Process p1;
            var sections = ExeInShell(WaitCmd(30));
            using (var manager = new ExternalProcessManager(sections.Item1, sections.Item2))
            {
                var task = Task.Run(() => manager.StartAndMonitorAsync());
                Thread.Sleep(2000); // Give some time for the process to start
                // process = Process.GetProcessesByName("cmd")[0];
                var id = manager.Process.Id;
                p1 = Process.GetProcessById(id);

                p1.Refresh();
                // Assert.IsTrue(process.Responding);
            }

            Thread.Sleep(1000); // Give a short time for the process to be terminated
            // Assert.Throws<InvalidOperationException>(() => process.Refresh());
            // var p2 = Process.GetProcessById(id);
            EnsureProcessExited(p1, false);
        }

        public static void EnsureProcessExited(Process process, bool normally = true)
        {
            if (!process.WaitForExit(0))
                throw new TimeoutException("Process did not exit within 0 seconds.");

            if (
                (normally && process.ExitCode != 0) ||
                (!normally && process.ExitCode == 0)
            )
                throw new AssertionException(
                    $"Process exited with code {process.ExitCode}\n" +
                    $"{process.StartInfo.FileName} {process.StartInfo.Arguments}");
        }
    }
}