/* eslint-disable no-restricted-imports */
import { test, expect, Page } from '@playwright/test';
import { ServiceFormEditPage } from '../pages/serviceFormEdit.page';
import { ServiceFormCreateNewPage } from '../pages/serviceFormCreateNew.page';
import { ServiceFormViewPage } from '../pages/serviceFormView.page';
import { insertTextToDraftJs } from '../utils/draftjs';

test('Should create new service with one language version and publish it succesfully', async ({ page }) => {
  const newServicePage = new ServiceFormCreateNewPage(page);

  // Create new service by adding Finnish language version.
  await newServicePage.goto();
  await newServicePage.languageVersionFiCheckbox.click();
  const serviceName = `Finnish Service test id ${Date.now()}`;
  await newServicePage.languageVersionFiInput.fill(serviceName);
  await newServicePage.confimButton.click();

  // Edit language version
  const serviceForm = new ServiceFormEditPage(page, 'fi');

  await expect(serviceForm.serviceName).toHaveText(serviceName);

  await serviceForm.inputName.isEditable();
  const editedServiceName = `${serviceName} edited`;
  await serviceForm.inputName.fill(editedServiceName);

  await serviceForm.summary.scrollIntoViewIfNeeded();
  await serviceForm.summary.isEditable();
  await serviceForm.summary.fill('Palvelun lyhyt tiivistelmä.');

  await serviceForm.description.scrollIntoViewIfNeeded();
  await insertTextToDraftJs(serviceForm.description, 'Palvelun kuvaus täytettynä.');

  await serviceForm.userInstructions.scrollIntoViewIfNeeded();
  await insertTextToDraftJs(serviceForm.userInstructions, 'Palvelun toimintaohjeet täydennettynä.');

  await serviceForm.conditions.scrollIntoViewIfNeeded();
  await insertTextToDraftJs(serviceForm.conditions, 'Palvelun ehdot ja kriteerit.');

  await serviceForm.selectLanguages.scrollIntoViewIfNeeded();
  await serviceForm.selectLanguages.focus();
  await page.keyboard.press('ArrowDown');
  await page.keyboard.press('Enter');

  await serviceForm.areaInformation.scrollIntoViewIfNeeded();
  await serviceForm.areaInformation.focus();

  await serviceForm.chargeType.scrollIntoViewIfNeeded();
  // Select first radiobutton in charge type
  await serviceForm.chargeType.locator('xpath=..').locator('label').first().check();

  await serviceForm.chargeInformation.scrollIntoViewIfNeeded();
  await insertTextToDraftJs(serviceForm.chargeInformation, 'Palvelun maksullisuustiedot täytettynä.');

  await serviceForm.targetGroups.first().scrollIntoViewIfNeeded();
  // Click on the first checkbox of the first targetgroup grouping (Citizens)
  await serviceForm.targetGroups.first().locator('.fi-checkbox').first().click();

  await serviceForm.serviceClassesOpenButton.scrollIntoViewIfNeeded();
  await serviceForm.serviceClassesOpenButton.click();
  await expect(page.locator('.fi-modal_title_focus-wrapper')).toHaveText('Select service groups');

  // Select two service classes from modal and click confirm
  await page.locator('button:has-text("Built environment")').click();
  await page.locator('label:has-text("Construction")').click();
  await page.locator('label:has-text("Built environment")').click();
  await page.locator('text=Confirm selection').click();

  await selectOntologyTermsAndKeywords(serviceForm, page);

  // Save as draft
  await Promise.all([serviceForm.clickFooterSaveDraft(), page.waitForNavigation()]);

  // Publish and transition to read view
  await serviceForm.clickHeaderPublish();
  await page.locator(`label:has-text("${serviceName}")`).click();
  await Promise.all([page.locator(`#publishing-dialog-publish`).click(), page.waitForNavigation()]);

  await assertPublishedService(page, editedServiceName);
});

async function assertPublishedService(page: Page, serviceName: string) {
  const serviceView = new ServiceFormViewPage(page, 'fi');

  await expect(serviceView.serviceNameHeader).toHaveText(serviceName);
  await expect(serviceView.languageVersionFiStatusText).toHaveText('Published');

  await serviceView.classificationAndKeywordsExpander.scrollIntoViewIfNeeded();
  await serviceView.classificationAndKeywordsExpander.click();
  await assertOntologyTermsAndKeywords(serviceView, page);
}

async function assertOntologyTermsAndKeywords(serviceView: ServiceFormViewPage, page: Page) {
  await serviceView.ontologyTermsHeader.scrollIntoViewIfNeeded();
  await expect(serviceView.ontologyTermsHeader.locator('ul span').first()).toHaveText('managers and executives');
  await expect(serviceView.ontologyTermsHeader.locator('ul span').nth(1)).toHaveText('software developers');

  await serviceView.keywordsHeader.scrollIntoViewIfNeeded();
  await expect(serviceView.keywordsHeader.locator('ul span').first()).toHaveText('keyword1');
  await expect(serviceView.keywordsHeader.locator('ul span').nth(1)).toHaveText('keyword2');
}

async function selectOntologyTermsAndKeywords(serviceForm: ServiceFormEditPage) {
  await serviceForm.ontologyTermsOpenButton.scrollIntoViewIfNeeded();
  await serviceForm.ontologyTermsOpenButton.click();
  await expect(serviceForm.page.locator('.fi-modal_title_focus-wrapper')).toHaveText('Select keywords yourself');

  // Search for ontology terms
  await serviceForm.page.locator('[placeholder="Enter and select a keyword"]').click();
  await serviceForm.page.locator('[placeholder="Enter and select a keyword"]').fill('Software developers');

  // Select from search results so that results are displayed
  await serviceForm.page.locator('text=software developers').click();

  // Select two terms
  await serviceForm.page.locator('label:has-text("software developers")').click();
  await serviceForm.page.locator('label:has-text("managers and executives")').click();

  // Switch to free keywords tab
  await serviceForm.page.locator('text=FREE KEYWORDS').click();

  // Add first free keyword
  await serviceForm.page.locator('[placeholder="Enter free keyword"]').click();
  await serviceForm.page.locator('[placeholder="Enter free keyword"]').fill('keyword1');
  await serviceForm.page.locator('button:has-text("Add as free keyword")').click();
  await serviceForm.page.locator('text=/.*Freely assigned keyword "keyword1" added.*/').waitFor();

  // Add second free keyword
  await serviceForm.page.locator('[placeholder="Enter free keyword"]').click();
  await serviceForm.page.locator('[placeholder="Enter free keyword"]').fill('keyword2');
  await serviceForm.page.locator('button:has-text("Add as free keyword")').click();
  await serviceForm.page.locator('text=/.*Freely assigned keyword "keyword2" added.*/').waitFor();

  // Close the modal
  await serviceForm.page.locator('button:has-text("Confirm selection")').click();
}
