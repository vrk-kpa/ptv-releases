enum MatomoCategories {
  ServiceForm = 'ServiceForm',
}

enum MatomoActionTypes {
  AddLanguages = 'AddLanguage',
  ServiceCopied = 'ServiceCopied',
  ServiceArchivedOrRestored = 'ServiceArchivedOrRestored',
  PublishSucceeded = 'PublishSucceeded',
  SaveSucceededPublishFailed = 'SaveSucceededPublishFailed',
  TranslationOrderSucceeded = 'TranslationOrderSucceeded',
  SaveDraftSucceeded = 'SaveDraftSucceeded',
  SaveDraftFailed = 'SaveDraftFailed',
  ResetForm = 'ResetForm',
}

export { MatomoCategories, MatomoActionTypes };
