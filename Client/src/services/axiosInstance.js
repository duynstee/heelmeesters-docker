import axios from 'axios';
import { getToken } from './authService';

const instance = axios.create({
  baseURL: '/api/'
});

instance.interceptors.request.use(config => {
  const token = getToken();
  if (token) config.headers['Authorization'] = `Bearer ${token}`;
  return config;
});

instance.interceptors.request.use(config => {
  console.log("REQ:", config.method?.toUpperCase(), config.baseURL + config.url);
  return config;
}); // tijdelijk voor debugging

export default instance;
