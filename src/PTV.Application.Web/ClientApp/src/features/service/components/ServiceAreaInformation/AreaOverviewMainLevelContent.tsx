import React from 'react';
import Box from '@mui/material/Box';
import { Municipality } from 'types/areaTypes';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { translateToLang } from 'utils/translations';

interface AreaOverviewMainLevelContentInterface {
  municipalities: Municipality[];
}

export default function AreaOverviewMainLevelContent(props: AreaOverviewMainLevelContentInterface): React.ReactElement | null {
  const uiLang = useGetUiLanguage();
  const translatedItems = props.municipalities.map((item) => {
    return translateToLang(uiLang, item.names) ?? '';
  });

  return (
    <Box>
      <Box>{translatedItems?.sort().join(', ')}</Box>
    </Box>
  );
}
