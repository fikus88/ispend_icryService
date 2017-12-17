// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BankData;
//
//    var data = Accounts.FromJson(jsonString);
//
namespace BankData
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class Accounts
    {
        [JsonProperty("results")]
        public List<AccountsResult> Results { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public partial class AccountsResult
    {
        [JsonProperty("update_timestamp")]
        public DateTime UpdateTimestamp { get; set; }

        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("account_type")]
        public string AccountType { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("account_number")]
        public AccountNumber AccountNumber { get; set; }

        [JsonProperty("provider")]
        public Provider Provider { get; set; }
    }

    public partial class Provider
    {
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("provider_id")]
        public string ProviderId { get; set; }

        [JsonProperty("logo_uri")]
        public string LogoUri { get; set; }
    }

    public partial class AccountNumber
    {
        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("swift_bic")]
        public string SwiftBic { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("sort_code")]
        public string SortCode { get; set; }
    }

    public partial class Accounts
    {
        public static Accounts FromJson(string json) => JsonConvert.DeserializeObject<Accounts>(json, AccountsConverter.Settings);
    }

    public static class AccountsSerialize
    {
        public static string ToJson(this Accounts self) => JsonConvert.SerializeObject(self, AccountsConverter.Settings);
    }

    public class AccountsConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}