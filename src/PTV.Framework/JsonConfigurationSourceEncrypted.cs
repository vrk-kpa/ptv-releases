using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace PTV.Framework
{
    public class JsonConfigurationSourceEncrypted : JsonConfigurationSource
    {
        public string EncryptionKey { get; set; }
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new EncryptedConfig(this, EncryptionKey);
        }
    }
}