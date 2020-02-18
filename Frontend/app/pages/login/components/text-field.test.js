import { createTestHarness } from '../../../test-utils/testHarness';

const setup = {
  component: {
    name: 'textField',
    path: 'pages/login/components/text-field.njk',
  },
};

describe('textField', () => {
  it('should render the label', createTestHarness(setup, (harness) => {
    const context = {
      params: {
        dataTestId: 'some-test-id',
        id: 'an-id',
        label: 'a label for the field',
      },
    };

    harness.request(context, ($) => {
      const textField = $('[data-test-id="some-test-id"]');
      expect(textField.find('label.nhsuk-label').text().trim()).toEqual(context.params.label);
    });
  }));

  it('should render the data', createTestHarness(setup, (harness) => {
    const context = {
      params: {
        dataTestId: 'some-test-id',
        id: 'an-id',
        data: 'data for the field',
      },
    };

    harness.request(context, ($) => {
      const textField = $('[data-test-id="some-test-id"]');
      expect(textField.find('input').val()).toEqual(context.params.data);
    });
  }));

  it('should add type password to the input field if set', createTestHarness(setup, (harness) => {
    const context = {
      params: {
        dataTestId: 'some-test-id',
        id: 'an-id',
        data: 'data for the field',
        type: 'password',
      },
    };

    harness.request(context, ($) => {
      const textField = $('[data-test-id="some-test-id"]');
      expect(textField.find('input').attr('type')).toEqual('password');
    });
  }));
});
