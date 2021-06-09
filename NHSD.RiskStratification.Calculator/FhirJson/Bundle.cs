#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NHSD.RiskStratification.Calculator.FhirJson
{
    public class Bundle : Resource
    {
        private List<EntryComponent>? _entry;

        [JsonProperty("type", Order = 1, NullValueHandling = NullValueHandling.Ignore)]
        public BundleType? Type { get; set; }

        [JsonProperty("entry", Order = 1)]
        public List<EntryComponent> Entry
        {
            get => _entry ??= new List<EntryComponent>();
            set => _entry = value;
        }

        /// <summary>
        /// Convention used by <see cref="Newtonsoft"/> to control serialisation of <see cref="Entry"/>
        /// </summary>
        public bool ShouldSerializeEntry() => _entry?.Count > 0;

        public EntryComponent AddResourceEntry(Resource resource, string? fullUrl)
        {
            var entryComponent = new EntryComponent
            {
                Resource = resource,
                FullUrl = fullUrl
            };

            Entry.Add(entryComponent);

            return entryComponent;
        }

        public sealed class EntryComponent
        {
            [JsonProperty("resource", NullValueHandling = NullValueHandling.Ignore)]
            public Resource? Resource { get; set; }

            [JsonProperty("fullUrl", NullValueHandling = NullValueHandling.Ignore)]
            public string? FullUrl { get; set; }
        }

        public IEnumerable<Resource> GetResources()
        {
            if (_entry != null)
            {
                return (from entry in Entry
                        where entry.Resource is { }
                        select entry.Resource)!;
            }

            return Array.Empty<Resource>();
        }

        [JsonConverter(typeof(JsonConverterFhirEnum<BundleType>))]
        public enum BundleType
        {
            Document,
            Message,
            Transaction,
            TransactionResponse,
            Batch,
            BatchResponse,
            History,
            Searchset,
            Collection
        }
    }
}
