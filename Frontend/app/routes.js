import express from 'express';
import logger from './logger';
import { errorHandler } from './pages/error/errorHandler';

const router = express.Router();

router.get('/healthcheck', async (req, res) => {
  logger.info('navigating to healthcheck page');
  res.send('Identity Server - Frontend - is Running!!!');
});

router.get('*', (req, res, next) => next({
  status: 404,
  message: 'Incorrect url - please check it is valid and try again',
}));

router.use((err, req, res, next) => {
  if (err) {
    const context = errorHandler(err);
    logger.error(context.message);
    return res.render('pages/error/template.njk', context);
  }
  return next();
});

module.exports = router;
