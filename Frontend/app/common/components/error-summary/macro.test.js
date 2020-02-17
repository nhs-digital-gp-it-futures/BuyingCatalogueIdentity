import { createTestHarness } from '../../../test-utils/testHarness';

const setup = {
  component: {
    name: 'errorSummary',
    path: 'common/components/error-summary/macro.njk',
  },
};

describe('errorSummary', () => {
  it('should render the error summary title', createTestHarness(setup, (harness) => {
    const context = {
      params: {
        errors: [],
      },
    };

    harness.request(context, ($) => {
      expect($('.nhsuk-error-summary h2').text().trim()).toEqual('There is a problem');
    });
  }));

  it('should render the error summary body', createTestHarness(setup, (harness) => {
    const context = {
      params: {
        errors: [],
      },
    };

    harness.request(context, ($) => {
      expect($('.nhsuk-error-summary p').text().trim()).toEqual('To complete this page, resolve the following errors;');
    });
  }));

  it('should render the one error if the context only contains a single error', createTestHarness(setup, (harness) => {
    const context = {
      params: {
        errors: [
          {
            text: 'This is the first error',
            href: '#link-to-first-error',
          },
        ],
      },
    };

    harness.request(context, ($) => {
      expect($('ul li a').text().trim()).toEqual('This is the first error');
      expect($('ul li a').attr('href')).toEqual('#link-to-first-error');
    });
  }));

  it('should render multiple errors if the context contains multiple errors', createTestHarness(setup, (harness) => {
    const context = {
      params: {
        errors: [
          {
            text: 'This is the first error',
            href: '#link-to-first-error',
          },
          {
            text: 'This is the second error',
            href: '#link-to-second-error',
          },
          {
            text: 'This is the third error',
            href: '#link-to-third-error',
          },
        ],
      },
    };

    harness.request(context, ($) => {
      expect($('ul li').length).toEqual(3);
    });
  }));
});
