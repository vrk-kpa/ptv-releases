// global-setup.ts
import { chromium, FullConfig, Page } from '@playwright/test';

async function globalSetup(config: FullConfig) {
  const browser = await chromium.launch();
  const page = await browser.newPage();

  await page.goto(`${config.projects[0].use.baseURL}/next/login`);

  // Wait for the page load to complete before proceeding. Otherwise e.g.
  // switching language does not work
  await page.locator('#login-button').waitFor();

  await switchUiLanguageToEnglish(page);

  // Fill username (email)
  await page.locator('input[id=name]').fill('test@dvv.fi');
  // Search for digi- ja väestö from organization selector
  await page.locator('input[role="combobox"]').fill('Digital and Population');
  // Press Enter
  await page.locator('input[role="combobox"]').press('Enter');
  // Select admin role
  await page.locator('text=PTV Administrator').click();

  // Click login button and wait for navigation
  await Promise.all([page.waitForNavigation(), page.locator('#login-button').click()]);

  // Save signed-in state to 'storageState.json'.
  await page.context().storageState({ path: 'storageState.json' });
  await browser.close();
}

async function switchUiLanguageToEnglish(page: Page) {
  const fiButtonSelector = 'button:has-text("Suomeksi (FI)")';
  const svButtonSelector = 'button:has-text("På svenska (SV")';
  const enTextSelector = 'text=In English (EN)';

  const inEnglish = await page.isVisible('button:has-text("In English (EN)")');
  if (inEnglish) return;

  const inFinnish = await page.isVisible(fiButtonSelector);
  if (inFinnish) {
    await page.locator(fiButtonSelector).click();
    await page.locator(enTextSelector).click();
    return;
  }

  const inSwedish = await page.isVisible(svButtonSelector);
  if (inSwedish) {
    await page.locator(svButtonSelector).click();
    await page.locator(enTextSelector).click();
    return;
  }

  throw new Error('Could not determine how to switch to english language');
}

export default globalSetup;
