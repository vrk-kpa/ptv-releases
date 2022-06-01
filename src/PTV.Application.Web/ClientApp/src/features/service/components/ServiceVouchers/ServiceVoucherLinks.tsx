import React, { FunctionComponent } from 'react';
import { Control, UseFormSetValue, UseFormTrigger, useController, useFieldArray } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Block, Button } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, ServiceVoucherLink, cLv, cService, cVoucher } from 'types/forms/serviceFormTypes';
import { Message } from 'components/Message';
import { VisualHeading } from 'components/VisualHeading';
import { toFieldStatus } from 'utils/rhf';
import { ServiceVoucherLinkItem } from './ServiceVoucherLinkItem';

const useStyles = makeStyles((theme) => ({
  fieldArray: {
    border: `1px solid ${theme.colors.archived}`,
    marginBottom: '20px',
  },
}));

interface IServiceVoucherLinks {
  tabLanguage: Language;
  control: Control<ServiceModel>;
  trigger: UseFormTrigger<ServiceModel>;
  setValue: UseFormSetValue<ServiceModel>;
}

function createNewLink(): ServiceVoucherLink {
  return {
    additionalInformation: '',
    name: '',
    orderNumber: 0,
    url: '',
  };
}

export const ServiceVoucherLinks: FunctionComponent<IServiceVoucherLinks> = (props: IServiceVoucherLinks): React.ReactElement => {
  const { t } = useTranslation();
  const classes = useStyles();

  const { fields, append, remove } = useFieldArray({
    control: props.control,
    name: `${cService.languageVersions}.${props.tabLanguage}.${cLv.voucher}.${cVoucher.links}`,
  });

  const { fieldState } = useController({
    control: props.control,
    name: `${cService.languageVersions}.${props.tabLanguage}.${cLv.voucher}.${cVoucher.linksErrorTag}`,
  });

  const { status, statusText } = toFieldStatus(fieldState);
  const showError = status === 'error';

  function addLink() {
    const index = fields.length;
    append(createNewLink());
    props.trigger([
      `${cService.languageVersions}.${props.tabLanguage}.${cLv.voucher}.${cVoucher.links}.${index}`,
      `${cService.languageVersions}.${props.tabLanguage}.${cLv.voucher}.${cVoucher.linksErrorTag}`,
    ]);
  }

  function removeLink(index: number) {
    remove(index);
    props.trigger(`${cService.languageVersions}.${props.tabLanguage}.${cLv.voucher}.${cVoucher.linksErrorTag}`);
  }

  return (
    <Box>
      <Box mb={2}>
        <VisualHeading variant='h4'>{t('Ptv.Service.Form.Field.ServiceVoucherLinks.Label')}</VisualHeading>
      </Box>
      {showError && (
        <Box mb={2}>
          <Block>
            <Message type='error'>{statusText}</Message>
          </Block>
        </Box>
      )}
      <Box>
        <Box>
          {fields.map((item, index) => {
            return (
              <div className={classes.fieldArray} key={item.id}>
                <Block padding='m'>
                  <ServiceVoucherLinkItem
                    language={props.tabLanguage}
                    control={props.control}
                    index={index}
                    onRemove={() => removeLink(index)}
                    setValue={props.setValue}
                  />
                </Block>
              </div>
            );
          })}
        </Box>
        <Button id={`add.voucher.${props.tabLanguage}`} type='button' onClick={addLink}>
          {t('Ptv.Service.Form.Field.ServiceVoucherLinks.AddNew.Label')}
        </Button>
      </Box>
    </Box>
  );
};
