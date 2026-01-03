import axios from 'axios';

const instance = axios.create({
    baseURL: 'https://localhost:7113/api',
    timeout: 5000, // 5 seconds timeout
});

export default instance;