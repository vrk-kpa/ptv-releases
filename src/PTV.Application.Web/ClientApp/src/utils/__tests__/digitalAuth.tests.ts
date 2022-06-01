import { DigitalAuthorizationModel } from 'types/digitalAuthTypes';
import { getTopLevelGroupsThatContain } from 'utils/digitalAuth';

function createGroup(id: string, childId: string): DigitalAuthorizationModel {
  return {
    id: id,
    isValid: true,
    code: 'anything',
    children: [
      {
        id: childId,
        isValid: true,
        code: 'anything',
        children: [],
        names: { en: 'anything' },
      },
    ],
    names: {
      en: 'anything',
    },
  };
}

describe('getTopLevelGroupsThatContain', () => {
  it('returns groups that contains authorizaton ids', () => {
    const authorizations: DigitalAuthorizationModel[] = [
      {
        id: 'root',
        isValid: true,
        code: 'anything',
        children: [createGroup('a', 'a-child'), createGroup('b', 'b-child'), createGroup('c', 'c-child')],
        names: {
          en: 'text',
        },
      },
    ];

    const result = getTopLevelGroupsThatContain(authorizations, ['a-child', 'c-child']);
    expect(result.length).toBe(2);
    expect(result[0].id).toBe('a');
    expect(result[1].id).toBe('c');
  });

  it('throws if root is empty list', () => {
    expect(() => getTopLevelGroupsThatContain([], [])).toThrow();
  });

  it('throws if root has more than one item', () => {
    const authorizations: DigitalAuthorizationModel[] = [
      {
        id: '1',
        isValid: true,
        code: 'a',
        children: [],
        names: {
          en: 'text',
        },
      },
      {
        id: '2',
        isValid: true,
        code: 'b',
        children: [],
        names: {
          en: 'text',
        },
      },
    ];

    expect(() => getTopLevelGroupsThatContain(authorizations, [])).toThrow();
  });
});
