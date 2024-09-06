#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MAVLinkPack.Editor.Util;
using Debug = UnityEngine.Debug;

namespace MAVLinkPack.Scripts.Util
{
    using System;
    using System.Threading;

    public class Retry<TI>
    {
        public List<TI> Attempts = null!;

        private ArgsT? _args;

        public ArgsT Args => _args ?? DefaultArgs;

        public string Name = "Retry-" + Retry.NameCounter.Next();

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
            Func<Exception, TI, bool>? shouldRetry = null,
            bool logException = false,
            string? name = null
        )

        {
            _args = new ArgsT
            {
                Interval = interval ?? DefaultArgs.Interval,
                ShouldContinue = shouldRetry ?? DefaultArgs.ShouldContinue,
                LogException = logException
            };

            if (name != null) Name = name;
            return this;
        }

        public struct ArgsT
        {
            public TimeSpan Interval;
            public Func<Exception, TI, bool> ShouldContinue;
            public bool LogException;
        }

        public class FixedIntervalT : Dependent<Retry<TI>>
        {
            public T Run<T>(Func<TI, TimeSpan, T> operation)
            {
                if (operation == null)
                    throw new ArgumentNullException(nameof(operation));

                var errors = new List<(TI, Exception)>();

                var stopwatch = Stopwatch.StartNew();

                var counter = 0;

                var zipped = Outer.Attempts.ZipWithNext().ToList();

                foreach (var (attempt, next) in zipped)
                {
                    try
                    {
                        return operation(attempt, stopwatch.Elapsed);
                    }
                    catch (Exception ex)
                    {
                        if (Outer.Args.LogException) Debug.LogException(ex);
                        errors.Add((attempt, ex));

                        var baseInfo =
                            $"{Outer.Name}/[{counter}/{zipped.Count}] {attempt}: Retry failed @ {stopwatch.Elapsed}s:" +
                            $"\n{ex.GetMessageForDisplay()}";

                        if (!Outer.Args.ShouldContinue(ex, attempt) || !next.HasValue)
                        {
                            Debug.Log(
                                baseInfo + $"\nthis is the last"
                            );

                            // augmenting error message
                            var info =
                                $"All {counter + 1} attempt(s) failed on [" +
                                $"{string.Join(", ", errors.Select(kv => kv.Item1))}" +
                                "]";

                            var ee = new RetryException(
                                info,
                                errors.Select(kv => kv.Item2)
                            );

                            throw ee;
                        }

                        Debug.Log(
                            baseInfo + $"\nwill try again at [{next.Value}]"
                        );

                        Thread.Sleep(Outer.Args.Interval);
                    }

                    counter += 1;
                }

                throw new SystemException("IMPOSSIBLE!");
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
        public static AtomicLong NameCounter = new();

        public static Retry<int> UpTo(int maxAttempts)
        {
            return new Retry<int>
            {
                Attempts = Enumerable.Range(0, maxAttempts).ToList()
            };
        }
    }


    public class RetryException : AggregateException
    {
        // constructors
        public RetryException(string message) : base(message)
        {
        }

        public RetryException(string message, IEnumerable<Exception> innerExceptions) : base(message,
            innerExceptions)
        {
        }
    }

    public static class RetryExtensions
    {
        public static Retry<TI> Retry<TI>(
            this IEnumerable<TI> attempts,
            string? name = null
        )
        {
            var result = new Retry<TI>
            {
                Attempts = attempts.ToList()
            };

            if (name != null) result.Name = name;

            return result;
        }
    }
}