import { padStart } from 'utils';

describe('padStart', () => {
  it('pads number with fillstring', () => {
    const result = padStart(1, 2, '0');
    expect(result).toBe('01');
  });

  it('pads string with fillstring', () => {
    const result = padStart('aa', 5, '0');
    expect(result).toBe('000aa');
  });
});
