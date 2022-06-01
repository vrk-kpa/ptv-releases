import React, { FunctionComponent } from 'react';
import { useTranslation } from 'react-i18next';
import { RhfReadOnlyField } from 'fields';
import { Block } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceVoucherLink, cLv, cService, cServiceVoucherLink, cVoucher } from 'types/forms/serviceFormTypes';
import { withNamespace } from 'utils/fieldIds';

interface IServiceVoucherLinkView {
  value: ServiceVoucherLink;
  index: number;
  language: Language;
}

export const ServiceVoucherLinkView: FunctionComponent<IServiceVoucherLinkView> = ({ language, value, index }) => {
  const { t } = useTranslation();
  const namespace = `${cService.languageVersions}.${language}.${cLv.voucher}.${cVoucher.links}`;

  return (
    <Block>
      <div>
        <RhfReadOnlyField
          value={value.name}
          id={withNamespace(namespace, `${index}.${cServiceVoucherLink.name}`)}
          labelText={t('Ptv.Service.Form.Field.ServiceVoucherLink.Name.Label')}
        />
      </div>
      <div>
        <RhfReadOnlyField
          value={value.url}
          id={withNamespace(namespace, `${index}.${cServiceVoucherLink.url}`)}
          labelText={t('Ptv.Service.Form.Field.ServiceVoucherLink.Url.Label')}
          asLink
        />
      </div>
      <div>
        <RhfReadOnlyField
          value={value.additionalInformation}
          id={withNamespace(namespace, `${index}.${cServiceVoucherLink.additionalInformation}`)}
          labelText={t('Ptv.Service.Form.Field.ServiceVoucherLink.AdditionalInformation.Label')}
        />
      </div>
    </Block>
  );
};
