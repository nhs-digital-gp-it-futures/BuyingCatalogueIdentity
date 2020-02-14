export const withCatch = route => async (req, res, next) => {
  try {
    return await route(req, res, next);
  } catch (err) {
    return next(err);
  }
};
