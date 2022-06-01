import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Button } from 'suomifi-ui-components';
import { ServiceFormValues } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { GdSearchItem } from 'hooks/queries/useSearchGeneralDescriptions';
import { UseGdModal } from './UseGdModal';

type FooterProps = {
  selectedItem: GdSearchItem | null | undefined;
  select: (gd: GeneralDescriptionModel, useGdName: boolean) => void;
  clearSelection: () => void;
  getFormValues: () => ServiceFormValues;
};

export function Footer(props: FooterProps): React.ReactElement {
  const { t } = useTranslation();
  const [isOpen, setIsOpen] = useState<boolean>(false);

  function select(gd: GeneralDescriptionModel, useGdName: boolean) {
    setIsOpen(false);
    props.select(gd, useGdName);
  }

  return (
    <div>
      <Button disabled={!props.selectedItem} onClick={() => setIsOpen(true)} key='use-gd'>
        {t('Ptv.Service.Form.GdSearch.UseGdButton.Label')}
      </Button>
      <Button
        disabled={!props.selectedItem}
        onClick={() => props.clearSelection()}
        icon='remove'
        key='clear-selection'
        variant='secondaryNoBorder'
      >
        {t('Ptv.Service.Form.GdSearch.ClearSelectedButton.Label')}
      </Button>
      {props.selectedItem && isOpen && (
        <UseGdModal
          getFormValues={props.getFormValues}
          select={select}
          searchResult={props.selectedItem}
          isOpen={isOpen}
          close={() => setIsOpen(false)}
        />
      )}
    </div>
  );
}
