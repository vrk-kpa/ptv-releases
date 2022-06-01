import { Locator } from '@playwright/test';

export const insertTextToDraftJs = async (locator: Locator, inputText: string) => {
  // locator fill would be way faster but with draftjs that doesn't seem to work.
  // Also page.keyboard.insertText is way faster, but that seems to work only in chromium based browsers
  await locator.click();
  await locator.type(inputText);
};
