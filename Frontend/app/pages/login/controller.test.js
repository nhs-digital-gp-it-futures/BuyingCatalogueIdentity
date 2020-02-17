import { getLoginPageContext } from './controller';
import manifest from './manifest.json';

describe('login page controller', () => {
  describe('getLoginPageContext', () => {
    it('returns the manifest values', () => {
      const context = getLoginPageContext();
      expect(Object.keys(context).length).toEqual(Object.keys(manifest).length);
      Object.keys(context).forEach((key) => {
        expect(context[key]).toEqual(manifest[key]);
      });
    });
  });
});
