import React from 'react';
import { Control, UseFormSetValue, useController } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import _ from 'lodash';
import { Paragraph } from 'suomifi-ui-components';
import { DigitalAuthorizationModel } from 'types/digitalAuthTypes';
import { AuthorizationModel, ConnectionFormModel, cC } from 'types/forms/connectionFormTypes';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { getTopLevelGroupsThatContain } from 'utils/digitalAuth';
import { localeCompareTexts } from 'utils/translations';
import { FormBlock } from 'features/connection/components/FormLayout';
import { RemovableItemBlock } from 'features/connection/components/RemovableItemBlock';
import { AuthorizationGroup } from './AuthorizationGroup';
import { AuthorizationSelector } from './AuthorizationSelector';

type AuthorizationsProps = {
  control: Control<ConnectionFormModel>;
  // with correct types TS takes too long
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  setValue: UseFormSetValue<any>;
};

export function Authorizations(props: AuthorizationsProps): React.ReactElement {
  const { t } = useTranslation();
  const lang = useGetUiLanguage();

  const digitalAuthorizations = useAppContextOrThrow().staticData.digitalAuthorizations;

  // Note we treat this list as a field instead of field array because it is quite difficult
  // to turn this into a model that works with RHF. It would need to be array inside
  // of array (groups and authorizations inside groups).
  const { field } = useController({
    name: `${cC.digitalAuthorizations}`,
    control: props.control,
  });

  const selectedIds = field.value.map((x) => x.id);

  const groups = getTopLevelGroupsThatContain(digitalAuthorizations, selectedIds).sort((left, right) =>
    localeCompareTexts(left.names, right.names, lang)
  );

  function removeGroup(groupId: string) {
    const group = groups.find((x) => x.id === groupId);
    if (!group) return;

    // Remove all ids which are part of the group
    const childIds = group.children.map((x) => x.id);
    const authModels: AuthorizationModel[] = field.value
      .filter((x) => !childIds.includes(x.id))
      .map((x) => {
        return { id: x.id };
      });

    props.setValue(cC.digitalAuthorizations, authModels);
  }

  function toggleAuthorization(id: string) {
    const addToList = field.value.find((x) => x.id === id) === undefined;
    if (addToList) {
      const authModels: AuthorizationModel[] = field.value.concat([{ id: id }]);
      props.setValue(cC.digitalAuthorizations, authModels);
      return;
    }

    const authModels: AuthorizationModel[] = field.value.filter((x) => x.id !== id);
    props.setValue(cC.digitalAuthorizations, authModels);
  }

  function toggleAuthorizationOrGroup(authModel: DigitalAuthorizationModel, isGroup: boolean, isSelected: boolean) {
    if (!isGroup) {
      toggleAuthorization(authModel.id);
      return;
    }

    if (isSelected) {
      removeGroup(authModel.id);
      return;
    }

    // User wants to select whole group but she could have already selected some of the authorizations
    // which belong to that group. Put everything into one list and just filter out duplicates
    const allIds: string[] = field.value.map((x) => x.id).concat(authModel.children.map((x) => x.id));
    const uniq: AuthorizationModel[] = _.uniq(allIds).map((x) => {
      return { id: x };
    });
    props.setValue(cC.digitalAuthorizations, uniq);
  }

  return (
    <div>
      <FormBlock marginBottom='20px'>
        <Paragraph>{t('Ptv.ConnectionDetails.Authorizations.Description')}</Paragraph>
      </FormBlock>

      <FormBlock marginBottom='20px'>
        <AuthorizationSelector selected={selectedIds} toggle={toggleAuthorizationOrGroup} />
      </FormBlock>

      {groups.map((item, index) => {
        return (
          <div key={item.id}>
            <RemovableItemBlock onRemove={() => removeGroup(item.id)}>
              <AuthorizationGroup group={item} selected={selectedIds} toggle={toggleAuthorization} />
            </RemovableItemBlock>
          </div>
        );
      })}
    </div>
  );
}
