#nullable enable
using System.Linq;
using MAVLinkPack.Scripts.Util;

namespace MAVLinkPack.Scripts.API
{
    using System;
    using System.Collections.Generic;

    public static class Subscriber
    {
        public class OnT<T> : Subscriber<Message<T>> where T : struct
        {
            protected override Indexed<Topic> Topics_Mk()
            {
                Indexed<Topic?> result = Indexed<Topic>.Global();
                result.Get<T>().Value = message => new List<Message<T>>
                {
                    Message<T>.FromRaw(message)
                };
                return result;
            }
        }

        public static OnT<T> On<T>() where T : struct
        {
            return new OnT<T>();
        }
    }


    public abstract class Subscriber<T>
    {
        public delegate List<T>? Topic(MAVLink.MAVLinkMessage message);

        public static readonly Topic? TopicMissing = _ => null;

        private Box<Indexed<Topic>>? _topics;

        private Indexed<Topic> Topics =>
            // Topics_Mk.BindToLazy(ref _topics); // TODO: oops, eta-expansion does't work.
            LazyHelper.EnsureInitialized(ref _topics, Topics_Mk);

        protected abstract Indexed<Topic> Topics_Mk();


        public List<T>? Process(MAVLink.MAVLinkMessage message)
        {
            return Topics.Get(message.msgid).ValueOr(TopicMissing)(message);
        }

        public abstract class BinaryT : Subscriber<T>
        {
            public Subscriber<T> Left = null!; // TODO: T in left and right can have different subtypes
            public Subscriber<T> Right = null!;
        }

        public class UnionT : BinaryT
        {
            protected override Indexed<Topic> Topics_Mk()
            {
                var merged = Left.Topics.Index.Merge(
                    Right.Topics.Index,
                    (ll, rr) => input =>
                    {
                        var before = (ll(input), rr(input));

                        return before.NullableReduce((l, r) => l.Union(r).ToList());
                    }
                );

                return Indexed<Topic>.Global(merged);
            }
        }

        public UnionT Union(Subscriber<T> that)
        {
            return new UnionT
            {
                Left = this,
                Right = that
            };
        }

        public class OrElseT : BinaryT
        {
            protected override Indexed<Topic> Topics_Mk()
            {
                var merged = Left.Topics.Index.Merge(
                    Right.Topics.Index,
                    (ll, rr) => input =>
                    {
                        var result = ll(input) ?? rr(input);

                        return result;
                    }
                );

                return Indexed<Topic>.Global(merged);
            }
        }

        public OrElseT OrElse(Subscriber<T> that)
        {
            return new OrElseT
            {
                Left = this,
                Right = that
            };
        }

        public class CutElimination<T2> : Subscriber<T2>
        {
            public Subscriber<T> Prev = null!;
            public Func<MAVLink.MAVLinkMessage, T, List<T2>> Fn = null!;

            protected override Indexed<Topic> Topics_Mk()
            {
                var oldTopics = Prev.Topics.Index;

                var newTopics = oldTopics.ToDictionary(
                    kv => kv.Key,
                    kv =>
                    {
                        Topic topic = (ii) =>
                        {
                            var prevV = kv.Value(ii);

                            if (prevV == null) return null;

                            var result = prevV.SelectMany(
                                x => Fn(ii, x)
                            ).ToList();

                            return result;
                        };
                        return topic;
                    }
                );

                return Indexed<Topic>.Global(newTopics);
            }
        }

        public CutElimination<T2> SelectMany<T2>(Func<MAVLink.MAVLinkMessage, T, List<T2>> fn)
        {
            return new CutElimination<T2>
            {
                Prev = this,
                Fn = fn
            };
        }

        public CutElimination<T2> Select<T2>(Func<MAVLink.MAVLinkMessage, T, T2> fn)
        {
            return SelectMany((ii, x) => new List<T2> { fn(ii, x) });
        }

        public Reader<T> LatchOn(MAVConnection mav)
        {
            return mav.Read(this);
        }
    }

    // public static class ProcessorExtensions
    // {
    //     public static Subscriber<T>? Add<T>(this Subscriber<T>? left, Subscriber<T>? right)
    //     {
    //         var result = (left, right).NullableReduce(
    //             (x, y) => new Subscriber<T>.UnionT { Left = x, Right = y }
    //         );
    //
    //         return result;
    //     }
    //
    //     public static Subscriber<T>? OrElse<T>(this Subscriber<T>? left, Subscriber<T>? right)
    //     {
    //         var result = (left, right).NullableReduce(
    //             (x, y) => new Subscriber<T>.OrElseT { Left = x, Right = y }
    //         );
    //
    //         return result;
    //     }
    // }
}