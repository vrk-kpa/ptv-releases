import React from 'react';
import { styled } from '@mui/material/styles';
import { Chip } from 'suomifi-ui-components';
import { GdChip } from './GdChip';

type KeywordChipListProps = {
  keywords: string[];
  toggleKeyword?: (keyword: string) => void;
};

const ChipContainer = styled('div')({
  display: 'inline-block',
  marginRight: '10px',
  marginTop: '10px',
});

export function KeywordChipList(props: KeywordChipListProps): React.ReactElement {
  const removable = !!props.toggleKeyword;

  return (
    <>
      {props.keywords.map((item) => {
        return (
          <ChipContainer key={item}>
            {removable ? (
              <Chip removable={removable} onClick={() => props.toggleKeyword?.(item)}>
                {item}
              </Chip>
            ) : (
              <GdChip className='custom'>{item}</GdChip>
            )}
          </ChipContainer>
        );
      })}
    </>
  );
}
