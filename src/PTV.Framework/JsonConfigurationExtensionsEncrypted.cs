using System;
using Microsoft.Extensions.Configuration;

namespace PTV.Framework
{
    public static class JsonConfigurationExtensionsEncrypted
    {
        public static IConfigurationBuilder AddJsonFileEncrypted(this IConfigurationBuilder builder, string path, string key, bool optional = false, bool reloadOnChange = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return builder;
            }
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path must be a non-empty string.");
            }

            var source = new JsonConfigurationSourceEncrypted
            {
                FileProvider = null,
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange,
                EncryptionKey = key
            };

            source.ResolveFileProvider();
            builder.Add(source);
            return builder;
        }
    }
}