import { test, expect } from '@playwright/test';
import { v4 as uuidv4 } from 'uuid';
// eslint-disable-next-line no-restricted-imports
import { ServiceFormEditPage } from '../pages/serviceFormEdit.page';
// eslint-disable-next-line no-restricted-imports
import { ServiceFormCreateNewPage } from '../pages/serviceFormCreateNew.page';

test.describe('Create new service', () => {
  const serviceId = uuidv4();
  let newServiceUrl = '';
  let newServiceNameFinnish = '';
  let newServiceNameEnglish = '';

  test('should create new service with finnish language version and save as draft', async ({ page }) => {
    const newServicePage = new ServiceFormCreateNewPage(page);
    await newServicePage.goto();

    await newServicePage.languageVersionFiCheckbox.click();
    await newServicePage.languageVersionFiInput.fill(`Finnish Service test id ${serviceId}`);
    await newServicePage.confimButton.click();

    const serviceForm = new ServiceFormEditPage(page);

    await Promise.all([serviceForm.clickHeaderSaveDraft(), page.waitForNavigation()]);

    newServiceNameFinnish = `Finnish Service test id ${serviceId}`;
    await expect(serviceForm.serviceName).toHaveText(newServiceNameFinnish);
    newServiceUrl = page.url();
  });

  test('saved service should open via url', async ({ page }) => {
    await page.goto(newServiceUrl);
    const servicePage = new ServiceFormEditPage(page);
    await expect(servicePage.serviceName).toHaveText(newServiceNameFinnish);
  });

  test('should be able to add new language version in english', async ({ page }) => {
    const newServicePage = new ServiceFormCreateNewPage(page);
    const serviceForm = new ServiceFormEditPage(page);
    await page.goto(newServiceUrl);

    // Add new language version
    await serviceForm.addLanguageButton.click();

    // Click english langauge
    await newServicePage.languageVersionEnCheckbox.click();
    newServiceNameEnglish = `English Service test id ${serviceId}`;
    // Fill english service name
    await newServicePage.languageVersionEnInput.fill(newServiceNameEnglish);
    // Confirm adding service
    await newServicePage.confimButton.click();

    await Promise.all([
      page.waitForNavigation(),
      // Save as draft
      serviceForm.clickHeaderSaveDraft(),
    ]);

    await expect(serviceForm.serviceName).toHaveText(newServiceNameEnglish);
  });
});
