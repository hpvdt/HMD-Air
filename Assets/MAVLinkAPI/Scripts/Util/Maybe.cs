using System;

namespace MAVLinkAPI.Scripts.Util
{
    // TODO: theoretically it is faster than Box<T>?, but difficult to use
    // TODO: should extend IEnumerable<T>

    public readonly struct Maybe<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;

        private Maybe(T value)
        {
            _value = value;
            _hasValue = true;
        }

        public static Maybe<T> Some(T value)
        {
            return new Maybe<T>(value);
        }

        public static Maybe<T> None()
        {
            return new Maybe<T>();
        }

        public bool HasValue => _hasValue;

        public T Value => _hasValue ? _value : throw new InvalidOperationException("Maybe does not have a value");

        public T ValueOrDefault(T defaultValue)
        {
            return _hasValue ? _value : defaultValue;
        }

        public void Match(Action<T> some, Action none)
        {
            if (_hasValue)
                some(_value);
            else
                none();
        }

        public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            return _hasValue ? some(_value) : none();
        }

        public Maybe<TResult> Map<TResult>(Func<T, TResult> map)
        {
            return _hasValue ? Maybe<TResult>.Some(map(_value)) : Maybe<TResult>.None();
        }

        public Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> bind)
        {
            return _hasValue ? bind(_value) : Maybe<TResult>.None();
        }

        public override string ToString()
        {
            return _hasValue ? $"Some({_value})" : "None";
        }

        public static implicit operator Maybe<T>(T value)
        {
            return Some(value);
        }
    }
}