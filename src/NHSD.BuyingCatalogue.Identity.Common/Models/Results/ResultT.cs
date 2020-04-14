using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NHSD.BuyingCatalogue.Identity.Common.Models.Results
{
    public sealed class Result<T>
    {
        public bool IsSuccess { get; }

        public IReadOnlyCollection<ErrorDetails> Errors { get; }

        [MaybeNull]
        public T Value { get; }

        internal Result(bool isSuccess, IEnumerable<ErrorDetails> errors, [AllowNull]T value)
        {
            IsSuccess = isSuccess;
            Errors = new ReadOnlyCollection<ErrorDetails>(errors != null ? errors.ToList() : new List<ErrorDetails>());
            Value = value!;
        }

        public Result ToResult()
        {
            if (IsSuccess)
                return Result.Success();
            
            return Result.Failure(Errors);
        }

        private static bool AreErrorsEqual(IEnumerable<ErrorDetails> first, IEnumerable<ErrorDetails> second)
        {
            if (first is null)
                return second is null;
            
            if (second is null)
                return false;

            return first.SequenceEqual(second);
        }

        private bool Equals(Result other)
        {
            return IsSuccess == other.IsSuccess && AreErrorsEqual(Errors, other.Errors);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is Result other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsSuccess, Errors);
        }
    }
}
