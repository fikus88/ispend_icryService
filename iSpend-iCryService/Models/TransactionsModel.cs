// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BankData;
//
//    var data = Transaction.FromJson(jsonString);
//
namespace BankData
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class Transactions
    {
        [JsonProperty("results")]
        public List<TransactionsResult> Results { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public partial class TransactionsResult
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("transaction_type")]
        public string TransactionType { get; set; }

        [JsonProperty("transaction_category")]
        public string TransactionCategory { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public partial class Meta
    {
        [JsonProperty("provider_transaction_category")]
        public string ProviderTransactionCategory { get; set; }

        [JsonProperty("transaction_reference")]
        public string TransactionReference { get; set; }
    }

    public partial class Transactions
    {
        public static Transactions FromJson(string json) => JsonConvert.DeserializeObject<Transactions>(json, TransactionsConverter.Settings);
    }

    public static class TransactionsSerialize
    {
        public static string ToJson(this Transactions self) => JsonConvert.SerializeObject(self, TransactionsConverter.Settings);
    }

    public class TransactionsConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}