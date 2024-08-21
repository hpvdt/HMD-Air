namespace MAVLinkAPI.Scripts.Util
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public class ExternalProcessManager : SafeClean
    {
        public readonly ProcessStartInfo StartInfo;
        public readonly Process Process;
        private CancellationTokenSource _cts;

        public ExternalProcessManager(string fileName, string arguments = "")
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = true,
                CreateNoWindow = false
                // RedirectStandardOutput = true,
                // RedirectStandardError = true
            };
            Process = new Process
            {
                StartInfo = StartInfo,
                EnableRaisingEvents = true
            };
            _cts = new CancellationTokenSource();
        }

        public async Task<bool> StartAndMonitorAsync()
        {
            if (!Process.Start())
                return false;

            await MonitorProcessAsync(_cts.Token);
            return true;
        }

        private async Task MonitorProcessAsync(CancellationToken token)
        {
            while (!Process.HasExited)
            {
                if (token.IsCancellationRequested)
                    break;

                if (await Task.Run(() => Process.WaitForExit(10000)))
                    break;

                if (!Process.Responding)
                {
                    Process.Kill();
                    break;
                }

                await Task.Delay(100, token);
            }
        }

        public void EnsureExited()
        {
            _cts.Cancel();

            if (!Process.HasExited)
            {
                Process.CloseMainWindow();
                if (!Process.WaitForExit(5000))
                {
                    UnityEngine.Debug.LogWarning("process did not exit after 5 seconds, killing process");
                    Process.Kill();
                }
            }

            Process?.Dispose();
        }

        public void Stop()
        {
            if (Process.HasExited) throw new Exception("Process has been exited");

            EnsureExited();
        }

        public string Info =>
            $"{Process.StartInfo.FileName} {Process.StartInfo.Arguments}\n" +
            $"===== Output =====\n" +
            $"{Process.StandardOutput.ReadToEnd()}\n" +
            $"===== Error! =====\n" +
            $"{Process.StandardError.ReadToEnd()}";

        protected override bool DoClean()
        {
            EnsureExited();
            _cts.Dispose();
            return true;
        }
    }
}