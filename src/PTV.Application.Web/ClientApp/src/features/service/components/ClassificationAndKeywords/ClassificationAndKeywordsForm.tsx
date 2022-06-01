import React from 'react';
import { Control, useWatch } from 'react-hook-form';
import Box from '@mui/material/Box';
import { Language } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { FormBlock } from 'components/formLayout/FormBlock';
import { FormDivider } from 'components/formLayout/FormDivider';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { getGdValueOrDefault } from 'utils/gd';
import { KeywordsAndOntologyTermsForm } from 'features/service/components/KeywordsAndOntologyTerms/EditMode';
import { ServiceClasses } from 'features/service/components/ServiceClasses';
import { ServiceIndustrialClasses } from 'features/service/components/ServiceIndustrialClasses';
import { ServiceLifeEvents } from 'features/service/components/ServiceLifeEvents';
import { TargetGroupSelector } from 'features/service/components/TargetGroupSelector';
import { containsAnyBusinessTargetGroup, containsAnyCitizenTargetGroup } from 'features/service/utils';

type ClassificationAndKeywordsFormProps = {
  language: Language;
  enabledLanguages: Language[];
  control: Control<ServiceModel>;
};

export function ClassificationAndKeywordsForm(props: ClassificationAndKeywordsFormProps): React.ReactElement {
  const appContext = useAppContextOrThrow();
  const namespace = `${cService.languageVersions}.${props.language}`;

  const serviceTargetGroups = useWatch({ control: props.control, name: `${cService.targetGroups}` });

  const generalDescription = useWatch({ control: props.control, name: `${cService.generalDescription}` });
  const gdKeywords = getGdValueOrDefault(generalDescription?.languageVersions, props.language, (x) => x.keywords, []);
  const gdOntologyTerms = generalDescription?.ontologyTerms ?? [];

  const gdTargetGroups = generalDescription?.targetGroups ?? [];
  const allTargetGroups = (serviceTargetGroups ?? []).concat(gdTargetGroups);

  return (
    <Box>
      <FormBlock>
        <TargetGroupSelector
          control={props.control}
          tabLanguage={props.language}
          allTargetGroups={appContext.staticData.targetGroups}
          gdTargetGroupIds={generalDescription?.targetGroups || []}
        />
        <FormDivider my={3} />
      </FormBlock>
      <FormBlock>
        <ServiceClasses gd={generalDescription} control={props.control} namespace={namespace} />
        <FormDivider my={3} />
      </FormBlock>
      <FormBlock>
        <KeywordsAndOntologyTermsForm
          namespace={namespace}
          control={props.control}
          gdKeywords={gdKeywords}
          gdOntologyTerms={gdOntologyTerms}
        />
        <FormDivider my={3} />
      </FormBlock>
      {containsAnyCitizenTargetGroup(allTargetGroups) && (
        <FormBlock>
          <ServiceLifeEvents control={props.control} gdItems={generalDescription?.lifeEvents || []} />
          <FormDivider my={3} />
        </FormBlock>
      )}
      {containsAnyBusinessTargetGroup(allTargetGroups) && (
        <FormBlock>
          <ServiceIndustrialClasses gd={generalDescription} control={props.control} namespace={namespace} />
        </FormBlock>
      )}
    </Box>
  );
}
