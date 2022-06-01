using Newtonsoft.Json;

namespace PTV.Localization.Services.Model
{
    public class StartTranslationFileDownloadRequest
    {
        [JsonProperty("data")]
        public ResouceTranslationRequestData Data { get; set; }
    }

    public class ResouceTranslationRequestData
    {
        [JsonProperty("attributes")]
        public Attributes Attributes { get; set; }
        
        [JsonProperty("relationships")]
        public Relationships Relationships { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; } = "resource_translations_async_downloads";
    }

    public class Attributes
    {
        [JsonProperty("content_encoding")]
        public string ContentEncoding { get; } = "text";
        [JsonProperty("file_type")]
        public string FileType { get; } = "json";
        [JsonProperty("mode")]
        public string Mode { get; set; }
    }
    
    public class Relationships
    {
        [JsonProperty("language")]
        public IdAndTypeData Language { get; set; }
        
        [JsonProperty("resource")]
        public IdAndTypeData Resource { get; set; }
    }

    public class IdAndTypeData
    {
        [JsonProperty("data")]
        public IdAndType Data { get; set; }
    }
    
    public class IdAndType
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
    }
    
    public class TranslationStatusResponse
    {
        [JsonProperty("data")]
        public TranslationStatusResponseData Data { get; set; }
    }
    
    public class TranslationStatusResponseData
    {
        [JsonProperty("attributes")]
        public TranslationStatusResponseAttributes Attributes { get; set; }
    }
    
    public class TranslationStatusResponseAttributes
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }
    
    public class TransifexLanguageValue
    {
        [JsonProperty("string")]
        public string Value { get; set; }
    }
    
    public class StartTranslationFileDownloadResponse
    {
        [JsonProperty("data")]
        public StartTranslationFileDownloadResponseData Data { get; set; }
    }

    public class StartTranslationFileDownloadResponseData
    {
        [JsonProperty("links")]
        public StartTranslationFileDownloadResponseLinks Links { get; set; }
    }

    public class StartTranslationFileDownloadResponseLinks
    {
        [JsonProperty("self")]
        public string Self { get; set; }
    }
}