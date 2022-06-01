# Playwright

This directory contains playwright tests.

Playwright documentation can be found from https://playwright.dev/docs

## How to run tests

Start local services (backend and frontends).

Example commands:

- Run all playwright tests with all defined browsers `npx playwright test`
- Run all tests from specific file e.g: `npx playwright test e2e/serviceform/simplePublish.spec.ts`
- Run tests in headed mode: `npx playwright test --headed`
- Run only with specific browser: `npx playwright test --project=firefox `
- Run code gen that records inputs: `npx playwright codegen`

Recommended VS Code extension that adds recording and test running to editor: https://marketplace.visualstudio.com/items?itemName=ms-playwright.playwright
