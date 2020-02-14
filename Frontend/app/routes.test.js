import request from 'supertest';
import { App } from '../app';
import routes from './routes';

jest.mock('./logger');

const mockPreviewContext = {
  solutionHeader: {
    id: '100000-001',
    name: 'Write on Time',
    supplierName: 'Really Kool Corporation',
    isFoundation: true,
    lastUpdated: '1996-03-15T10:00:00',
  },
  returnToDashboardUrl: '/supplier/solution/100000-001',
  sections: {
    'solution-description': { answers: {} },
    features: { answers: {} },
    'contact-details': { answers: {} },
    capabilities: { answers: {} },
  },
};

describe('GET /healthcheck', () => {
  it('should return the correct status and text', () => {
    const app = new App().createApp();
    app.use('/', routes);

    return request(app)
      .get('/healthcheck')
      .expect(200)
      .then((res) => {
        expect(res.text).toBe('Identity Server - Frontend - is Running!!!');
      });
  });
});

describe('Error handler', () => {
});

describe('GET *', () => {
  it('should return error page if url cannot be matched', () => {
    const app = new App().createApp();
    app.use('/', routes);
    return request(app)
      .get('/aaaa')
      .expect(200)
      .then((res) => {
        expect(res.text.includes('<h1 class="nhsuk-heading-l nhsuk-u-padding-left-3" data-test-id="error-page-title">Error: Incorrect url - please check it is valid and try again</h1>')).toEqual(true);
      });
  });
});
