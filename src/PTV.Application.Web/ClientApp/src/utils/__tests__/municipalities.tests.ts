import { Municipality } from 'types/areaTypes';
import { hasPostalCode } from 'utils/municipalities';

const municipalities: Municipality[] = [
  {
    id: '123',
    code: '123',
    descriptions: { fi: 'anything' },
    names: { fi: 'anything' },
    postalCodes: [
      {
        code: '00123',
        names: { fi: 'anything' },
      },
    ],
  },
  {
    id: '091',
    code: '091',
    descriptions: { fi: 'anything' },
    names: { fi: 'anything' },
    postalCodes: [
      {
        code: '00100',
        names: { fi: 'anything' },
      },
    ],
  },
  {
    id: '456',
    code: '456',
    descriptions: { fi: 'anything' },
    names: { fi: 'anything' },
    postalCodes: [
      {
        code: '00456',
        names: { fi: 'anything' },
      },
    ],
  },
];

describe('hasPostalCode', () => {
  it('returns true if postal code is found', () => {
    const result = hasPostalCode('091', municipalities, '00100');
    expect(result).toBe(true);
  });
  it('returns false if postal code is not found', () => {
    const result = hasPostalCode('091', municipalities, '00200');
    expect(result).toBe(false);
  });
  it('returns false if municipality is not found', () => {
    const result = hasPostalCode('999', municipalities, '00100');
    expect(result).toBe(false);
  });
});
