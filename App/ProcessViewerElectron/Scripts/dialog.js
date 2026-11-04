import { createApp } from 'vue'
import { createPinia } from 'pinia'
import DialogPage from '../Pages/DialogPage.vue'

import '../Styles/bootstrap/bootstrap.min.css';
import '../Styles/site.css';
import '../Scripts/bootstrap/js/bootstrap.bundle.min.js';

const app = createApp(DialogPage);
app.use(createPinia());
app.mount('#app');