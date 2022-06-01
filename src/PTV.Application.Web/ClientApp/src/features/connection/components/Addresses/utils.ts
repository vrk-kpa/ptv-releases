import _ from 'lodash';
import { Municipality } from 'types/areaTypes';
import { PostalCode } from 'types/postalCodeTypes';

export type PostalCodeSelectOption = {
  value: string;
  label: string;
  sortValue: number;
};

export function toPostalCodeOption(source: PostalCode): PostalCodeSelectOption {
  return {
    label: source.code,
    value: source.code,
    sortValue: parseInt(source.code, 10),
  };
}

export function searchPostalCodes(
  municipalities: Municipality[],
  userInput: string,
  selectedPostalCode: string | null
): PostalCodeSelectOption[] {
  const searchTerm = userInput.toLowerCase() || selectedPostalCode?.toLowerCase() || '';
  const result: PostalCodeSelectOption[] = [];
  for (const municipality of municipalities) {
    for (const postalCode of municipality.postalCodes) {
      if (searchTerm && postalCode.code.includes(searchTerm)) {
        result.push(toPostalCodeOption(postalCode));
      }
    }
  }

  return _.take(
    _.sortBy(result, (x) => x.sortValue),
    100
  );
}
