import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { Button, Modal, ModalContent, ModalFooter, ModalTitle } from 'suomifi-ui-components';
import { Region } from 'types/areaTypes';
import { useFormMetaContext } from 'context/formMeta';
import AreaOverviewMainLevel from './AreaOverviewMainLevel';

interface AreaOverviewInterface {
  items: Region[];
  overviewLabel: string;
  buttonOpenText: string;
}

export default function AreaOverview(props: AreaOverviewInterface): React.ReactElement | null {
  const { t } = useTranslation();
  const [isOpen, setIsOpen] = useState(false);

  function onClose() {
    setIsOpen(false);
  }

  function renderAreaOverviewMainLevel(item: Region) {
    return <AreaOverviewMainLevel key={item.id} item={item} />;
  }

  const { mode } = useFormMetaContext();
  if (mode === 'view') {
    return null;
  }

  return (
    <Box>
      <Box mb={0} mt={1}>
        <Button icon='infoFilled' variant='secondaryNoBorder' onClick={() => setIsOpen(true)}>
          {props.buttonOpenText}
        </Button>
      </Box>

      <Modal appElementId='root' visible={isOpen} onEscKeyDown={onClose}>
        <ModalContent>
          <ModalTitle>{props.overviewLabel}</ModalTitle>
          {props.items.map((code) => renderAreaOverviewMainLevel(code))}
        </ModalContent>
        <ModalFooter>
          <Button onClick={onClose}>{t('Ptv.Service.Form.Field.AreaOverview.Button.Close.Text')}</Button>
        </ModalFooter>
      </Modal>
    </Box>
  );
}
