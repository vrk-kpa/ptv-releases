import React from 'react';
import { styled } from '@mui/material/styles';
import { Chip } from 'suomifi-ui-components';
import { OntologyTerm } from 'types/classificationItemsTypes';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { getTextByLangPriority } from 'utils/translations';
import { GdChip } from './GdChip';

type OntologyTermChipListProps = {
  ontologyTerms: OntologyTerm[];
  toggleOntologyTerm?: (term: OntologyTerm) => void;
};

const ChipContainer = styled('div')({
  display: 'inline-block',
  marginRight: '10px',
  marginTop: '10px',
});

export function OntologyTermChipList(props: OntologyTermChipListProps): React.ReactElement {
  const uiLang = useGetUiLanguage();
  const removable = !!props.toggleOntologyTerm;
  return (
    <>
      {props.ontologyTerms.map((item) => {
        return (
          <ChipContainer key={item.id}>
            <>
              {removable ? (
                <Chip removable={removable} onClick={() => props.toggleOntologyTerm?.(item)}>
                  {getTextByLangPriority(uiLang, item.names)}
                </Chip>
              ) : (
                <GdChip className='custom'>{getTextByLangPriority(uiLang, item.names)}</GdChip>
              )}
            </>
          </ChipContainer>
        );
      })}
    </>
  );
}
