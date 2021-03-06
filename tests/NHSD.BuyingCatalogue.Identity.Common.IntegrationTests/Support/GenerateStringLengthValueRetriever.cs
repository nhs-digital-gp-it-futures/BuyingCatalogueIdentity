﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Common.IntegrationTests.Support
{
    public sealed class GenerateStringLengthValueRetriever : IValueRetriever
    {
        private const string PatternMatchGroupKey = "StringLength";

        private static readonly Regex SubstituteStringPattern =
            new(@$"#A string of length (?<{PatternMatchGroupKey}>\d+)#", RegexOptions.IgnoreCase);

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType) =>
            propertyType == typeof(string) && SubstituteStringPattern.IsMatch(keyValuePair.Value);

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType) =>
            Parse(keyValuePair.Value);

        private static string Parse(string value) => SubstituteStringPattern.Replace(value, OnMatch);

        private static string OnMatch(Match match)
        {
            string returnValue = match.Value;

            var matchedValue = match.Groups[PatternMatchGroupKey];
            if (int.TryParse(matchedValue.Value, NumberStyles.Integer, new NumberFormatInfo(), out var stringLength))
            {
                returnValue = new string('a', stringLength);
            }

            return returnValue;
        }
    }
}
