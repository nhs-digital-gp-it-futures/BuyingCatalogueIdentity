using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.SharedMocks
{
    public class MockRequestCookieCollection : IRequestCookieCollection
    {
        private readonly IDictionary<string, string> dictionary;

        public MockRequestCookieCollection(IDictionary<string, string> dictionary)
        {
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public int Count => dictionary.Count;

        public ICollection<string> Keys => dictionary.Keys;

#nullable enable
        public string? this[string key] => dictionary[key];
#nullable disable

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool ContainsKey(string key) => dictionary.ContainsKey(key);

#nullable enable
        public bool TryGetValue(string key, out string? value) => dictionary.TryGetValue(key, out value);
#nullable disable
    }
}
