import { Selector } from 'testcafe';
import { extractInnerText } from '../../test-utils/helper';
import manifest from './manifest.json';

const pageSetup = async (t) => {
  await t.navigateTo('http://localhost:1234/login');
};

fixture('Show Log in Page');

test('should display the page title', async (t) => {
  await pageSetup(t);
  const pageTitle = Selector('h1[data-test-id="login-page-title"]');
  await t
    .expect(pageTitle.exists).ok()
    .expect(await extractInnerText(pageTitle)).eql(manifest.title);
});

test('should display the page description', async (t) => {
  await pageSetup(t);
  const pageDescription = Selector('h2[data-test-id="login-page-description"]');
  await t
    .expect(pageDescription.exists).ok()
    .expect(await extractInnerText(pageDescription)).eql(manifest.description);
});

test('should display the email field', async (t) => {
  await pageSetup(t);
  const emailField = Selector('div[data-test-id="email-field"]');
  await t
    .expect(emailField.exists).ok()
    .expect(emailField.find('input').exists).ok()
    .expect(await extractInnerText(emailField.find('label'))).eql(manifest.emailFieldTextLabel);
});

test('should display the password field', async (t) => {
  await pageSetup(t);
  const passwordField = Selector('[data-test-id="password-field"]');
  await t
    .expect(passwordField.exists).ok()
    .expect(passwordField.find('input').exists).ok()
    .expect(passwordField.find('input').getAttribute('type')).eql('password')
    .expect(await extractInnerText(passwordField.find('label'))).eql(manifest.passwordFieldTextLabel);
});

test('should display the forgotten password link', async (t) => {
  await pageSetup(t);
  const passwordLink = Selector('[data-test-id="forgot-password-link"]');
  await t
    .expect(passwordLink.exists).ok()
    .expect(await extractInnerText(passwordLink)).eql(manifest.passwordResetLinkText)
    // .expect(passwordLink.getAttribute('href')).eql('');
});

test('should display the log in button', async (t) => {
  await pageSetup(t);
  const button = Selector('button');
  await t
    .expect(button.exists).ok()
    .expect(await extractInnerText(button)).eql(manifest.buttonText);
});

test('should display the request account link', async (t) => {
  await pageSetup(t);
  const requestAccountLink = Selector('[data-test-id="request-account-link"]');
  await t
    .expect(requestAccountLink.exists).ok()
    .expect(await extractInnerText(requestAccountLink)).eql(manifest.requestAccountLinkText);
  // .expect(requestAccountLink.getAttribute('href')).eql('');
});
