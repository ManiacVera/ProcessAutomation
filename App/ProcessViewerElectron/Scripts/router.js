import { createRouter, createWebHashHistory } from 'vue-router';
import ProcessListView from '../Pages/ProcessListView.vue';
import ProcessDetailView from '../Pages/ProcessDetailView.vue';

const routes = [
  {
    path: '/',
    redirect: '/ProcessList'
  },
  {
    path: '/ProcessList',
    name: 'ProcessList',
    component: ProcessListView
  },
  {
    path: '/ProcessDetail',
    name: 'ProcessDetail',
    component: ProcessDetailView,
    props: true
  }
];

const router = createRouter({
  history: createWebHashHistory(),
  routes
});

export default router;