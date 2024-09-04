#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HMD.Scripts.Util;
using Debug = UnityEngine.Debug;

namespace MAVLinkPack.Scripts.Util
{
    using System;
    using System.Threading;

    public class Retry<TI>
    {
        public IEnumerable<TI> Attempts = null!;

        private ArgsT? _args;

        public ArgsT Args => _args ?? DefaultArgs;

        private static bool DefaultShouldRetry(Exception ex, TI attempt)
        {
            return true;
        }

        private static readonly ArgsT DefaultArgs = new()
        {
            Interval = TimeSpan.FromSeconds(1),
            ShouldContinue = DefaultShouldRetry
        };

        public Retry<TI> With(
            TimeSpan? interval = null,
            Func<Exception, TI, bool>? shouldRetry = null
        )
        {
            _args = new ArgsT
            {
                Interval = interval ?? DefaultArgs.Interval,
                ShouldContinue = shouldRetry ?? DefaultArgs.ShouldContinue
            };
            return this;
        }

        public struct ArgsT
        {
            public TimeSpan Interval;
            public Func<Exception, TI, bool> ShouldContinue;
        }

        public class FixedIntervalT : Dependent<Retry<TI>>
        {
            public T Run<T>(Func<TI, TimeSpan, T> operation)
            {
                if (operation == null)
                    throw new ArgumentNullException(nameof(operation));

                var errors = new Dictionary<TI, Exception>();

                var stopwatch = Stopwatch.StartNew();

                var counter = 0;

                var zipped = Outer.Attempts.ZipWithNext(default);

                foreach (var (attempt, next) in zipped)
                    try
                    {
                        counter += 1;
                        return operation(attempt, stopwatch.Elapsed);
                    }
                    catch (Exception ex)
                    {
                        if (!Outer.Args.ShouldContinue(ex, attempt))
                        {
                            // augmenting error message
                            var addendum =
                                $"All {counter} attempt(s) failed\n" +
                                string.Join("\n", errors.Select(kv => $"    {kv.Key}: {kv.Value.Message}"));

                            ex.Data["Retry"] += addendum;

                            throw;
                        }

                        Debug.Log(
                            $"Error at `{attempt}` after {stopwatch.Elapsed} second(s): {ex.Message}\n" +
                            $"will try again at `{next}`"
                        );

                        Thread.Sleep(Outer.Args.Interval);

                        errors[attempt] = ex;
                        // TODO: Claude 3.5 should add offset for execution time already spent, not cool
                    }

                throw new Exception("IMPOSSIBLE!");
            }

            public void Run(Action<TI, TimeSpan> operation)
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

        public FixedIntervalT FixedInterval => new()
        {
            Outer = this
        };
    }

    public static class Retry
    {
        public static Retry<int> UpTo(int maxAttempts)
        {
            return new Retry<int>
            {
                Attempts = Enumerable.Range(0, maxAttempts)
            };
        }
    }

    public static class RetryExtensions
    {
        public static Retry<TI> Retry<TI>(
            this IEnumerable<TI> attempts
        )
        {
            return new Retry<TI>
            {
                Attempts = attempts
            };
        }
    }
}