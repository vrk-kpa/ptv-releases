import React from 'react';
import { Language } from 'types/enumTypes';
import { cLv } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { ComparisonView } from 'components/ComparisonView';
import {
  useFormMetaContext,
  useGetCompareFieldId,
  useGetCompareFieldName,
  useGetFieldId,
  useGetFieldName,
  useGetSelectedLanguage,
} from 'context/formMeta';
import { getGdValueOrDefault } from 'utils/gd';
import { BackgroundDescription } from './BackgroundDescription';

type ServiceBackgroundDescriptionInterface = {
  gd: GeneralDescriptionModel | null | undefined;
};

export function ServiceBackgroundDescription(props: ServiceBackgroundDescriptionInterface): React.ReactElement | null {
  const meta = useFormMetaContext();
  const language = useGetSelectedLanguage();

  const name = useGetFieldName();
  const compareName = useGetCompareFieldName();
  const id = useGetFieldId();
  const compareId = useGetCompareFieldId();

  const gd = props.gd;
  if (!gd) {
    return null;
  }

  function getGdBackgroundDescription(gd: GeneralDescriptionModel, lang: Language | undefined): string | null {
    return getGdValueOrDefault(gd.languageVersions, lang, (x) => x.backgroundDescription, null);
  }

  return (
    <ComparisonView
      left={
        <BackgroundDescription
          name={name(cLv.backgroundDescription)}
          id={id(cLv.backgroundDescription)}
          value={getGdBackgroundDescription(gd, language)}
        />
      }
      right={
        <BackgroundDescription
          name={compareName(cLv.backgroundDescription, meta.compareLanguageCode)}
          id={compareId(cLv.backgroundDescription, meta.compareLanguageCode)}
          value={getGdBackgroundDescription(gd, meta.compareLanguageCode)}
        />
      }
    />
  );
}
