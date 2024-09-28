namespace MAVLinkAPI.Scripts.Util
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public class ExternalProcessManager : IDisposable
    {
        public Process _process;
        private CancellationTokenSource _cts;

        public ExternalProcessManager(string fileName, string arguments = "")
        {
            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = true,
                    CreateNoWindow = false
                    // RedirectStandardOutput = true,
                    // RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };
            _cts = new CancellationTokenSource();
        }

        public async Task<bool> StartAndMonitorAsync()
        {
            if (!_process.Start())
                return false;

            await MonitorProcessAsync(_cts.Token);
            return true;
        }

        private async Task MonitorProcessAsync(CancellationToken token)
        {
            while (!_process.HasExited)
            {
                if (token.IsCancellationRequested)
                    break;

                if (await Task.Run(() => _process.WaitForExit(10000)))
                    break;

                if (!_process.Responding)
                {
                    _process.Kill();
                    break;
                }

                await Task.Delay(100, token);
            }
        }

        public void EnsureExited()
        {
            _cts.Cancel();

            if (!_process.HasExited)
            {
                _process.CloseMainWindow();
                if (!_process.WaitForExit(5000))
                {
                    UnityEngine.Debug.LogWarning("process did not exit after 5 seconds, killing process");
                    _process.Kill();
                }
            }

            _process?.Dispose();
        }

        public void Stop()
        {
            if (_process.HasExited) throw new Exception("Process has been exited");

            EnsureExited();
        }

        public void Dispose()
        {
            EnsureExited();
            _cts.Dispose();
        }

        public string Info =>
            $"{_process.StartInfo.FileName} {_process.StartInfo.Arguments}\n" +
            $"===== Output =====\n" +
            $"{_process.StandardOutput.ReadToEnd()}\n" +
            $"===== Error! =====\n" +
            $"{_process.StandardError.ReadToEnd()}";
    }
}