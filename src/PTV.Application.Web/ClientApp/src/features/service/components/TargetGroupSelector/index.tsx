import React, { useEffect, useState } from 'react';
import { Control, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { union } from 'lodash';
import { Heading, Paragraph } from 'suomifi-ui-components';
import { AuthorityTargetGroup, BusinessTargetGroup, CitizenTargetGroup } from 'types/constants';
import { Language } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { TargetGroup } from 'types/targetGroupTypes';
import { useFormMetaContext } from 'context/formMeta';
import { useGdSpecificTranslationKey } from 'hooks/translations/useGdSpecificTranslationKey';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { GeneralTargetGroupSelector } from './GeneralTargetGroupSelector';
import { getSelectedForGroup, getTargetGroups } from './utils';

type TargetGroupSelectorProps = {
  allTargetGroups: TargetGroup[];
  gdTargetGroupIds: string[];
  tabLanguage: Language;
  control: Control<ServiceModel>;
};

export function TargetGroupSelector(props: TargetGroupSelectorProps): React.ReactElement {
  const uiLang = useGetUiLanguage();
  const idPrefix = `${cService.languageVersions}.${props.tabLanguage}`;

  const { field } = useController({
    control: props.control,
    name: `${cService.targetGroups}`,
  });

  const { t } = useTranslation();
  const { allTargetGroups, gdTargetGroupIds: originalGdTargetGroupIds } = props;
  const { mode } = useFormMetaContext();
  const [gdIds, setGdIds] = useState<string[]>([]);
  const selected = union(field.value, gdIds);

  useEffect(() => setGdIds(originalGdTargetGroupIds), [originalGdTargetGroupIds]);

  function select(codes: string[]) {
    const currentSelection = union(selected, gdIds, codes).sort();
    field.onChange(currentSelection);
  }

  function unselect(codes: string[]) {
    gdIds.length > 0 && setGdIds(gdIds.filter((code) => !codes.includes(code)));
    const currentSelection = selected.filter((code) => !codes.includes(code));
    field.onChange(currentSelection);
  }

  const citizenTargetGroups = getTargetGroups(allTargetGroups, CitizenTargetGroup, uiLang, true);
  const businessTargetGroups = getTargetGroups(allTargetGroups, BusinessTargetGroup, uiLang, false);
  const authorityTargetGroups = getTargetGroups(allTargetGroups, AuthorityTargetGroup, uiLang, true);

  const hintKey = useGdSpecificTranslationKey(
    props.control,
    'Ptv.Service.Form.TargetGroups.Title.Description',
    'Ptv.Service.Form.TargetGroups.Title.GdSelected.Description'
  );

  return (
    <div>
      <Heading variant='h4' id={`${cService.languageVersions}.${props.tabLanguage}.${field.name}`} tabIndex={0}>
        {t('Ptv.Service.Form.TargetGroups.Title.Text')}
      </Heading>
      {mode === 'edit' && (
        <Box mt={2}>
          <Paragraph>{t(hintKey)}</Paragraph>
        </Box>
      )}
      <Box mt={2}>
        <GeneralTargetGroupSelector
          title={t('Ptv.Service.Form.TargetGroups.Citizens.Title')}
          hint={t('Ptv.Service.Form.TargetGroups.Citizens.Description')}
          targetGroups={citizenTargetGroups}
          selected={getSelectedForGroup(selected, CitizenTargetGroup)}
          select={select}
          unselect={unselect}
          id={`${idPrefix}.${field.name}`}
        />

        <GeneralTargetGroupSelector
          title={t('Ptv.Service.Form.TargetGroups.Businesses.Title')}
          targetGroups={businessTargetGroups}
          selected={getSelectedForGroup(selected, BusinessTargetGroup)}
          select={select}
          unselect={unselect}
          id={`${idPrefix}.${field.name}`}
        />

        <GeneralTargetGroupSelector
          title={t('Ptv.Service.Form.TargetGroups.Authorities.Title')}
          targetGroups={authorityTargetGroups}
          selected={getSelectedForGroup(selected, AuthorityTargetGroup)}
          select={select}
          unselect={unselect}
          id={`${idPrefix}.${field.name}`}
        />
      </Box>
    </div>
  );
}
