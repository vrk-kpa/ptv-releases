export const LingsoftRuleUrl = '1';
export const LingsoftRuleEmail = '2';
export const LingsoftRulePassiveVerb = '3';
export const LingsoftRuleCounterPart = '4';
export const LingsoftRuleTiivistelma = '5';
export const LingsoftRuleDuplicatesBaseText = '6';
export const LingsoftRuleLongSentence = '7';
export const LingsoftRuleLaws = '8';
export const LingsoftRuleGrammarError = '9';
export const LingsoftRuleTypingError = '10';
export const LingsoftRuleAddress = '11';
export const LingsoftRulePhoneNumber = '12';
export const LingsoftRuleName = '13';
export const LingsoftRuleOpeningHours = '14';
export const LingsoftRuleDate = '15';

export const lingsoftRule = [
  LingsoftRulePassiveVerb,
  LingsoftRuleCounterPart,
  LingsoftRuleLongSentence,
  LingsoftRuleGrammarError,
  LingsoftRuleTypingError,
] as const;

export type LingsoftRule = typeof lingsoftRule[number];

export const lingsoftRulePriority = [
  LingsoftRuleGrammarError,
  LingsoftRuleTypingError,
  LingsoftRuleUrl,
  LingsoftRuleEmail,
  LingsoftRuleAddress,
  LingsoftRulePhoneNumber,
  LingsoftRuleTiivistelma,
  LingsoftRuleDuplicatesBaseText,
  LingsoftRulePassiveVerb,
  LingsoftRuleCounterPart,
  LingsoftRuleLongSentence,
  LingsoftRuleLaws,
  LingsoftRuleName,
  LingsoftRuleOpeningHours,
  LingsoftRuleDate,
] as const;
