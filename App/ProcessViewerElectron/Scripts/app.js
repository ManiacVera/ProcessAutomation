import { createApp } from 'vue';
import { createPinia } from 'pinia'
import MasterView from '../Pages/MasterView.vue';
import router from './router';
import Vue3Toastify from 'vue3-toastify'

import 'vue3-toastify/dist/index.css'
import '../Styles/bootstrap/bootstrap.min.css';
import '../Styles/site.css';
import '../Scripts/bootstrap/js/bootstrap.bundle.min.js';

const app = createApp(MasterView);
app.use(createPinia());
app.use(router);
app.use(Vue3Toastify, {
  autoClose: 3000,
  position: 'bottom-right',
});
app.mount('#app');