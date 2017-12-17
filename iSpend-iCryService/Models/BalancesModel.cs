// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BankData;
//
//    var data = Balances.FromJson(jsonString);
//
namespace BankData
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class Balances
    {
        [JsonProperty("results")]
        public List<BalancesResult> Results { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public partial class BalancesResult
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("available")]
        public double Available { get; set; }

        [JsonProperty("current")]
        public double Current { get; set; }

        [JsonProperty("update_timestamp")]
        public DateTime UpdateTimestamp { get; set; }
    }

    public partial class Balances
    {
        public static Balances FromJson(string json) => JsonConvert.DeserializeObject<Balances>(json, BalancesConverter.Settings);
    }

    public static class BalancesSerialize
    {
        public static string ToJson(this Balances self) => JsonConvert.SerializeObject(self, BalancesConverter.Settings);
    }

    public class BalancesConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}