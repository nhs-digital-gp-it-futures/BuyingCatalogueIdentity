import axios from 'axios';
import { apiHost } from './config';
import logger from './logger';

export class ApiProvider {
  constructor() {
    this.apiHost = apiHost;
  }
}
