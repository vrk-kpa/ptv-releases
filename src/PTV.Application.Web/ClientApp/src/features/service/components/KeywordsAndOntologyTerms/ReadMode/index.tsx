import React from 'react';
import { useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { ViewValueList } from 'fields';
import { Block } from 'suomifi-ui-components';
import { cLv, cService } from 'types/forms/serviceFormTypes';
import { HeadingWithTooltip } from 'components/HeadingWithTooltip';
import { useFormMetaContext } from 'context/formMeta';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { getTextByLangPriority } from 'utils/translations';
import { KeywordsAndOntologyTermsProps } from 'features/service/components/KeywordsAndOntologyTerms/types';

export function KeywordsAndOntologyTermsReadView(props: KeywordsAndOntologyTermsProps): React.ReactElement {
  const uiLang = useGetUiLanguage();
  const { selectedLanguageCode } = useFormMetaContext();
  const { t } = useTranslation();

  const tooltipKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.Field.OntologyTerms.Display.Tooltip',
    'Ptv.Service.Form.Field.OntologyTerms.Display.GdSelected.Tooltip'
  );

  const { field: ontologyTermsField } = useController({
    control: props.control,
    name: `${cService.ontologyTerms}`,
  });

  const { field: keywordsField } = useController({
    control: props.control,
    name: `${cService.languageVersions}.${selectedLanguageCode}.${cLv.keywords}`,
  });

  const allKeywords = keywordsField.value.concat(props.gdKeywords).sort((a, b) => a.localeCompare(b));
  const allOntologyTerms = ontologyTermsField.value
    .concat(props.gdOntologyTerms)
    .map((x) => getTextByLangPriority(uiLang, x.names) ?? '')
    .sort((a, b) => a.localeCompare(b));

  return (
    <Block>
      <HeadingWithTooltip
        variant='h4'
        tooltipContent={t(tooltipKey)}
        tooltipAriaLabel={t('Ptv.Service.Form.Field.OntologyTerms.Display.Label')}
      >
        {t('Ptv.Service.Form.Field.OntologyTerms.Display.Label')}
      </HeadingWithTooltip>
      <Box mt={2}>
        <ViewValueList
          id={`${props.namespace}.${cService.ontologyTerms}`}
          labelText={t('Ptv.Service.Form.Field.OntologyTerms.Display.Selection.Label')}
          values={allOntologyTerms}
        />
      </Box>
      <Box mt={2}>
        <ViewValueList
          id={`${props.namespace}.${cLv.keywords}`}
          labelText={t('Ptv.Service.Form.Field.FreeKeywords.Display.Selection.Label')}
          values={allKeywords}
        />
      </Box>
    </Block>
  );
}
