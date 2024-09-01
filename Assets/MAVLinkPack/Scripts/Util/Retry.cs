#nullable enable
using System.Diagnostics;

namespace MAVLinkPack.Scripts.Util
{
    using System;
    using System.Threading;

    public static class Retry
    {
        private static bool DefaultShouldRetry(Exception ex, int attempt) => true;

        public static Args Of(
            int maxAttempts = 3,
            TimeSpan? interval = null,
            Func<Exception, int, bool>? shouldRetry = null
        )
        {
            if (maxAttempts <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxAttempts),
                    "Max attempts must be greater than zero.");


            return new Args
            {
                MaxAttempts = maxAttempts,
                Interval = interval ?? TimeSpan.FromSeconds(1),
                ShouldRetry = shouldRetry ?? DefaultShouldRetry
            };
        }

        public struct Args
        {
            public int MaxAttempts;
            public TimeSpan Interval;
            public Func<Exception, int, bool> ShouldRetry;


            // public static Args Default => new Args
            // {
            //     MaxAttempts = 3,
            //     Interval = TimeSpan.FromSeconds(1),
            //     ShouldRetry = ((ex, attempt) => true)
            // };

            public FixedInterval FixedInterval => new FixedInterval { Args = this };
        }

        public class FixedInterval
        {
            public Args Args;

            public T Run<T>(Func<int, TimeSpan, T> operation)
            {
                if (operation == null)
                    throw new ArgumentNullException(nameof(operation));

                var attempts = 0;
                var stopwatch = Stopwatch.StartNew();

                while (true)
                {
                    try
                    {
                        attempts++;
                        return operation(attempts, stopwatch.Elapsed);
                    }
                    catch (Exception ex)
                    {
                        if (attempts >= Args.MaxAttempts ||
                            !Args.ShouldRetry(ex, attempts))
                        {
                            throw;
                        }

                        Thread.Sleep(Args.Interval);
                        // TODO: Claude 3.5 should add offset for execution time already spent, not cool
                    }
                }
            }

            public void Run(Action<int, TimeSpan> operation)
            {
                if (operation == null)
                    throw new ArgumentNullException(nameof(operation));

                Run<object>((attempt, elapsed) =>
                {
                    operation(attempt, elapsed);
                    return null!;
                });
            }
        }
    }
}