import React, { FunctionComponent, useContext, useState } from 'react';
import { SearchInput, SearchInputProps } from 'suomifi-ui-components';
import { ClassificationItemsContext } from 'features/service/contexts/ClassificationItems/ClassificationItemsContextProvider';
import { DispatchContext } from 'features/service/contexts/ClassificationItems/DispatchContextProvider';
import { updateQuery } from 'features/service/contexts/ClassificationItems/actions';

interface SearchFilterInterface extends SearchInputProps {
  charCount?: number;
}

export const SearchFilter: FunctionComponent<SearchFilterInterface> = ({
  charCount = 3,
  labelText,
  visualPlaceholder,
  labelMode,
  searchButtonLabel,
  clearButtonLabel,
}) => {
  const [value, setValue] = useState('');
  const dispatch = useContext(DispatchContext);
  const { searchValue } = useContext(ClassificationItemsContext);
  const handleOnChange = (value: string | number | undefined) => {
    if (typeof value === 'string') {
      setValue(value);
      value.length >= charCount ? updateQuery(dispatch, value) : updateQuery(dispatch, '');
    }
  };

  return (
    <SearchInput
      onChange={handleOnChange}
      value={searchValue || value}
      searchButtonLabel={searchButtonLabel}
      clearButtonLabel={clearButtonLabel}
      labelText={labelText}
      labelMode={labelMode}
      visualPlaceholder={visualPlaceholder}
    />
  );
};
