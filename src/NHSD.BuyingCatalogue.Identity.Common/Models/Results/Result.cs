using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NHSD.BuyingCatalogue.Identity.Common.Models.Results
{
    public sealed class Result
    {
        private static readonly Result _success = new Result();

        public bool IsSuccess { get; }

        public IReadOnlyCollection<ErrorMessage> Errors { get; }

        private Result()
        {
            IsSuccess = true;
            Errors = new ReadOnlyCollection<ErrorMessage>(new List<ErrorMessage>());
        }

        private Result(IEnumerable<ErrorMessage> errors)
        {
            IsSuccess = false;
            Errors = new ReadOnlyCollection<ErrorMessage>(errors != null ? errors.ToList() : new List<ErrorMessage>());
        }

        public static Result Success()
        {
            return _success;
        }

        public static Result<T> Success<T>(T value)
        {
            return new Result<T>(true, default!, value);
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
