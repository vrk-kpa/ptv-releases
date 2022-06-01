using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PTV.Framework;
using PTV.Localization.Services.Model;

namespace PTV.Localization.Services.Handlers
{
    [RegisterService(typeof(TransifexDownloadHandler), RegisterType.Transient)]
    [RegisterService(typeof(ITranslationDataHandler), RegisterType.Transient)]
    public class TransifexDownloadHandler : TranslationDataHandlerBase
    {
        private TransifexApiHandler apiHandler;

        public TransifexDownloadHandler(TransifexApiHandler apiHandler)
        {
            this.apiHandler = apiHandler;
        }

        public override string Key => "download";

        protected override ITranslationData ExecuteInternal(string languageCode, ITranslationData data,
            ITranslationOptions options)
        {
            var keys = apiHandler.DownloadTranslations(languageCode);
            if (keys != null)
            {
                data.SetData(languageCode, keys);
            }

            return data;
        }
    }
}
