import React, { useState } from 'react';
import Box from '@mui/material/Box';
import { Expander, ExpanderContent, ExpanderTitleButton } from 'suomifi-ui-components';
import { Region } from 'types/areaTypes';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { translateToLang } from 'utils/translations';
import AreaOverviewMainLevelContent from './AreaOverviewMainLevelContent';

interface AreaOverviewMainLevelInterface {
  item: Region;
}
export default function AreaOverviewMainLevel(props: AreaOverviewMainLevelInterface): React.ReactElement | null {
  const uiLang = useGetUiLanguage();
  const [open, setOpen] = useState(false);
  const referenceItem = props.item;

  function onOpenChange() {
    setOpen((expanded) => !expanded);
  }

  function renderHeader(item: Region) {
    return <span>{`${translateToLang(uiLang, item.names) ?? ''} (${item.municipalities.length})`}</span>;
  }

  return (
    <Box>
      <Expander key={referenceItem.id} open={open} onOpenChange={onOpenChange} id={referenceItem.id}>
        <ExpanderTitleButton asHeading='h3'>{renderHeader(referenceItem)}</ExpanderTitleButton>
        {open && (
          <ExpanderContent>
            <AreaOverviewMainLevelContent municipalities={referenceItem.municipalities} />
          </ExpanderContent>
        )}
      </Expander>
    </Box>
  );
}
