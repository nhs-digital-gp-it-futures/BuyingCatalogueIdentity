using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.BuyingCatalogue.Identity.Common.Messages;

namespace NHSD.BuyingCatalogue.Identity.Common.Results
{
    public sealed class Result<T>
    {
        public bool IsSuccess { get; }

        public IReadOnlyCollection<ErrorMessage> Errors { get; }

        [MaybeNull]
        public T Value { get; }

        internal Result(bool isSuccess, IEnumerable<ErrorMessage>? errors, T value)
        {
            IsSuccess = isSuccess;
            Errors = new ReadOnlyCollection<ErrorMessage>(errors != null ? errors.ToList() : new List<ErrorMessage>());
            Value = value;
        }

        public Result ToResult()
        {
            if (IsSuccess)
                return Result.Success();

            return Result.Failure(Errors);
        }

        private static bool AreErrorsEqual(IEnumerable<ErrorMessage> first, IEnumerable<ErrorMessage> second)
        {
            if (first is null)
                return second is null;

            if (second is null)
                return false;

            return first.SequenceEqual(second);
        }

        private bool Equals(Result<T> other)
        {
            return other is object
                && IsSuccess == other.IsSuccess
                && AreErrorsEqual(Errors, other.Errors)
                && Equals(Value, other.Value);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is Result<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsSuccess, Errors, Value!);
        }
    }
}
