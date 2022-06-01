import React from 'react';
import { Control, UseFormSetValue, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Expander, ExpanderContent, ExpanderTitleButton } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, LanguageVersionExpanderTypes, StateLanguageVersionExpander } from 'types/forms/connectionFormTypes';
import { FormDivider } from 'components/formLayout/FormDivider';
import { Addresses } from 'features/connection/components/Addresses';
import { Authorizations } from 'features/connection/components/Authorizations';
import { BasicInformation } from 'features/connection/components/BasicInformation';
import { EmailAddresses } from 'features/connection/components/EmailAddresses';
import { ExceptionalServiceHours } from 'features/connection/components/ExceptionalServiceHours';
import { FaxNumbers } from 'features/connection/components/FaxNumbers';
import { HolidayServiceHours } from 'features/connection/components/HolidayServiceHours';
import { PhoneNumbers } from 'features/connection/components/PhoneNumbers';
import { SpecialServiceHours } from 'features/connection/components/SpecialServiceHours';
import { StandardServiceHours } from 'features/connection/components/StandardServiceHours';
import { WebPages } from 'features/connection/components/WebPages';
import { useConnectionContext } from 'features/connection/context';

const useStyles = makeStyles((theme) => ({
  item: {
    marginTop: '30px',
    marginBottom: '30px',
  },
}));

type BasicInformationProps = {
  language: Language;
  control: Control<ConnectionFormModel>;
  trigger: UseFormTrigger<ConnectionFormModel>;
  setValue: UseFormSetValue<ConnectionFormModel>;
  expanderStates: StateLanguageVersionExpander;
  toggleExpander: (states: LanguageVersionExpanderTypes) => void;
};

export function LanguageVersionForm(props: BasicInformationProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const channelType = useConnectionContext().connection.channelType;
  const showAdditionalFields = channelType === 'EChannel' || channelType === 'ServiceLocation' || channelType === 'Phone';

  return (
    <div>
      <Expander
        open={props.expanderStates['BasicInfo']}
        onOpenChange={() => {
          props.toggleExpander('BasicInfo');
        }}
      >
        <ExpanderTitleButton>{t('Ptv.ConnectionDetails.BasicInformation.Title')}</ExpanderTitleButton>
        <ExpanderContent>
          <BasicInformation control={props.control} setValue={props.setValue} />
        </ExpanderContent>
      </Expander>

      {showAdditionalFields && (
        <Expander
          open={props.expanderStates['ServiceHours']}
          onOpenChange={() => {
            props.toggleExpander('ServiceHours');
          }}
        >
          <ExpanderTitleButton>{t('Ptv.ConnectionDetails.ServiceHours.Title')}</ExpanderTitleButton>
          <ExpanderContent>
            <StandardServiceHours control={props.control} trigger={props.trigger} />
            <FormDivider className={classes.item} />
            <SpecialServiceHours control={props.control} trigger={props.trigger} />
            <FormDivider className={classes.item} />
            <HolidayServiceHours control={props.control} trigger={props.trigger} />
            <FormDivider className={classes.item} />
            <ExceptionalServiceHours control={props.control} trigger={props.trigger} />
          </ExpanderContent>
        </Expander>
      )}

      {showAdditionalFields && (
        <Expander
          open={props.expanderStates['ContactInformation']}
          onOpenChange={() => {
            props.toggleExpander('ContactInformation');
          }}
        >
          <ExpanderTitleButton>{t('Ptv.ConnectionDetails.ContactInformation.Title')}</ExpanderTitleButton>
          <ExpanderContent>
            <EmailAddresses control={props.control} trigger={props.trigger} />
            <FormDivider className={classes.item} />
            <PhoneNumbers control={props.control} trigger={props.trigger} />
            <FormDivider className={classes.item} />
            <Addresses control={props.control} trigger={props.trigger} />
            <FormDivider className={classes.item} />
            <WebPages control={props.control} trigger={props.trigger} />
            <FormDivider className={classes.item} />
            <FaxNumbers control={props.control} trigger={props.trigger} />
          </ExpanderContent>
        </Expander>
      )}

      <Expander
        open={props.expanderStates['Authorization']}
        onOpenChange={() => {
          props.toggleExpander('Authorization');
        }}
      >
        <ExpanderTitleButton>{t('Ptv.ConnectionDetails.Authorizations.Title')}</ExpanderTitleButton>
        <ExpanderContent>
          <Authorizations control={props.control} setValue={props.setValue} />
        </ExpanderContent>
      </Expander>
    </div>
  );
}
