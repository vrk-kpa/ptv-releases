import { LanguageVersionType } from 'types/languageVersionTypes';
import { hasItems } from 'utils/languageVersions';

type Item = {
  value: string;
};

const data: LanguageVersionType<Item[]> = {
  en: [{ value: '1' }],
  fi: [],
};

describe('hasItems', () => {
  it('returns true if language specific array has items', () => {
    const result = hasItems(data, 'en');
    expect(result).toBe(true);
  });

  it('returns false if language specific key does not exist', () => {
    const result = hasItems(data, 'sv');
    expect(result).toBe(false);
  });

  it('returns false if language specific key contains empty array', () => {
    const result = hasItems(data, 'fi');
    expect(result).toBe(false);
  });
});
