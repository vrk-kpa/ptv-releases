import React, { useState } from 'react';
import { Control, UseFormSetValue, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { Expander, ExpanderContent, ExpanderTitleButton } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { OptionalFieldByWatch } from 'components/OptionalFieldByWatch';
import { useFormMetaContext } from 'context/formMeta';
import { SelfProducerSelect } from './SelfProducerSelect';
import { SelfProducers } from './SelfProducers';

type SelfProducersMainLevelProps = {
  tabLanguage: Language;
  control: Control<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
  defaultSelfProducers: OrganizationModel[];
  namespace: string;
};

export function SelfProducersMainLevel(props: SelfProducersMainLevelProps): React.ReactElement {
  const { mode } = useFormMetaContext();
  const { t } = useTranslation();
  const [open, setOpen] = useState(false);

  const hasSelfProducers = useWatch({ control: props.control, name: `${cService.hasSelfProducers}` });
  const selfProducers = useWatch({ control: props.control, name: `${cService.selfProducers}` });

  function onOpenChange() {
    setOpen((expanded) => !expanded);
  }

  function setSelfProducers(organizations: OrganizationModel[]) {
    props.setValue(`${cService.selfProducers}`, organizations);
  }

  const count = selfProducers.length;

  if (mode === 'view') {
    return <SelfProducers control={props.control} name={cService.selfProducers} defaultItems={props.defaultSelfProducers} />;
  }

  return (
    <Box>
      <Expander id={cService.selfProducers} open={open} onOpenChange={onOpenChange}>
        <ExpanderTitleButton asHeading='h3'>
          <span>{t('Ptv.Service.Form.Field.ServiceProducers.SelfProducers.Title') + ` (${count})`}</span>
        </ExpanderTitleButton>
        <ExpanderContent>
          <Box>
            <SelfProducerSelect
              control={props.control}
              tabLanguage={props.tabLanguage}
              defaultSelfProducers={props.defaultSelfProducers}
              setSelfProducers={setSelfProducers}
              hasSelfProducers={hasSelfProducers}
            />
            <OptionalFieldByWatch<boolean>
              shouldRender={() => hasSelfProducers}
              control={props.control}
              fieldName={`${cService.hasSelfProducers}`}
            >
              <SelfProducers control={props.control} name={cService.selfProducers} defaultItems={props.defaultSelfProducers} />
            </OptionalFieldByWatch>
          </Box>
        </ExpanderContent>
      </Expander>
    </Box>
  );
}
