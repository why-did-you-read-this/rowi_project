import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import axios from 'axios'
import './index.css'


axios.defaults.baseURL = 'http://localhost:5077'
createApp(App).use(router).mount('#app')
