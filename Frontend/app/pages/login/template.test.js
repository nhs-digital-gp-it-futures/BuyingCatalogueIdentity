import { createTestHarness } from '../../test-utils/testHarness';
import context from './manifest.json';

const setup = {
  template: {
    path: 'pages/login/template.njk',
  },
};

describe('login page', () => {
  it('should render the login page title', createTestHarness(setup, (harness) => {
    harness.request(context, ($) => {
      const title = $('h1[data-test-id="login-page-title"]');
      expect(title.length).toEqual(1);
      expect(title.text().trim()).toEqual(context.title);
    });
  }));

  it('should render the login page description', createTestHarness(setup, (harness) => {
    harness.request(context, ($) => {
      const description = $('h2[data-test-id="login-page-description"]');
      expect(description.length).toEqual(1);
      expect(description.text().trim()).toEqual(context.description);
    });
  }));

  it('should render an email textField component', createTestHarness(setup, (harness) => {
    harness.request(context, ($) => {
      const emailTextField = $('[data-test-id="email-field"]');
      expect(emailTextField.length).toEqual(1);
      expect(emailTextField.text().trim()).toEqual(context.emailFieldTextLabel);
    });
  }));

  it('should render a password textField component', createTestHarness(setup, (harness) => {
    harness.request(context, ($) => {
      const emailTextField = $('[data-test-id="password-field"]');
      expect(emailTextField.length).toEqual(1);
      expect(emailTextField.text().trim()).toEqual(context.passwordFieldTextLabel);
      expect($(emailTextField).find('input').attr('type')).toEqual('password');
    });
  }));

  it('should render a forgotten password viewDataLink component', createTestHarness(setup, (harness) => {
    harness.request(context, ($) => {
      const passwordLink = $('[data-test-id="forgot-password-link"]');
      expect(passwordLink.length).toEqual(1);
      expect(passwordLink.text().trim()).toEqual(context.passwordResetLinkText);
      // TODO: add in when we have the link
      // expect($(passwordLink).find('a').attr('href')).toEqual('');
    });
  }));

  it('should render a login button', createTestHarness(setup, (harness) => {
    harness.request(context, ($) => {
      const button = $('button');
      expect(button.length).toEqual(1);
      expect(button.text().trim()).toEqual(context.buttonText);
      expect(button.hasClass('nhsuk-button--reverse')).toBeTruthy();
      // TODO: add in when we have the functionality
      // expect($(passwordLink).find('a').attr('href')).toEqual('');
    });
  }));

  it('should render a forgotten password viewDataLink component', createTestHarness(setup, (harness) => {
    harness.request(context, ($) => {
      const requestAccountLink = $('[data-test-id="request-account-link"]');
      expect(requestAccountLink.length).toEqual(1);
      expect(requestAccountLink.text().trim()).toEqual(context.requestAccountLinkText);
      // TODO: add in when we have the link
      // expect($(requestAccountLink).find('a').attr('href')).toEqual('');
    });
  }));
});
