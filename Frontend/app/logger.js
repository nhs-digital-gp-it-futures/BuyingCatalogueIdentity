import { createLogger, format, transports } from 'winston';
import { loggerLevel, env } from './config';

const {
  combine,
  timestamp,
  label,
  printf,
  colorize,
} = format;
const logFormat = printf(info => `${info.timestamp} [${info.level}] ${info.label} | message: ${info.message} ${info.message.stack ? `: ${info.message.stack}` : ''}`);

const logger = createLogger({
  format: combine(
    label({ label: 'identity-frontend' }),
    timestamp(),
    colorize(),
    logFormat,
  ),
  transports: [
    new transports.Console({
      level: loggerLevel,
      colourize: (env === 'development'),
    }),
  ],
});

export default logger;
