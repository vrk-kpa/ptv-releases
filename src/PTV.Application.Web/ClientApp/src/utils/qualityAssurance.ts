import { escapeRegExp } from 'lodash';

export function matchingTokenRegExp(token: string): RegExp {
  // Match the matchToken only as a separate word.
  // Separate words have either unicode white space \p{Z} or \p{P} punctuation between them,
  // or they are at the beginning or the of the string.
  return new RegExp(`(^|[\\p{Z}\\p{P}])${escapeRegExp(token)}([\\p{Z}\\p{P}]|$)`, 'u');
}
