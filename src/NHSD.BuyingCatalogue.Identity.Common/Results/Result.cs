using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NHSD.BuyingCatalogue.Identity.Common.Messages;

namespace NHSD.BuyingCatalogue.Identity.Common.Results
{
    public sealed class Result : IEquatable<Result>
    {
        private static readonly Result _success = new Result();

        public bool IsSuccess { get; }

        public IReadOnlyCollection<ErrorMessage> Errors { get; }

        private Result() : this(true, Enumerable.Empty<ErrorMessage>())
        {
        }

        private Result(IEnumerable<ErrorMessage> errors) : this(false, errors)
        {
        }

        private Result(bool isSuccess, IEnumerable<ErrorMessage>? errors)
        {
            IsSuccess = isSuccess;
            Errors = new ReadOnlyCollection<ErrorMessage>(errors != null ? errors.ToList() : new List<ErrorMessage>());
        }

        public static Result Success()
        {
            return _success;
        }

        public static Result<T> Success<T>(T value)
        {
            return new Result<T>(true, default, value);
        }

        public static Result Failure(IEnumerable<ErrorMessage> errors)
        {
            return new Result(errors);
        }

        public static Result<T> Failure<T>(IEnumerable<ErrorMessage> errors)
        {
            return new Result<T>(false, errors, default!);
        }

        private static bool AreErrorsEqual(IEnumerable<ErrorMessage> first, IEnumerable<ErrorMessage> second)
        {
            if (first is null)
                return second is null;
            
            if (second is null)
                return false;

            return first.SequenceEqual(second);
        }

        public bool Equals(Result other)
        {
            return other is object
                && IsSuccess == other.IsSuccess 
                && AreErrorsEqual(Errors, other.Errors);
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
