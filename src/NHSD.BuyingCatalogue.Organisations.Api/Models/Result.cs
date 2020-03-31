using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NHSD.BuyingCatalogue.Organisations.Api.Models
{
    public sealed class Result
    {
        private static readonly Result _success = new Result();

        public bool IsSuccess { get; }

        public IReadOnlyCollection<Error> Errors { get; }

        private Result()
        {
            IsSuccess = true;
        }

        private Result(IEnumerable<Error> errors)
        {
            IsSuccess = false;
            Errors = new ReadOnlyCollection<Error>(errors != null ? errors.ToList() : new List<Error>());
        }

        public static Result Success()
        {
            return _success;
        }

        public static Result Failure(IEnumerable<Error> errors)
        {
            return new Result(errors);
        }

        private static bool AreErrorsEqual(IEnumerable<Error> first, IEnumerable<Error> second)
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

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is Result other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsSuccess, Errors);
        }
    }
}
