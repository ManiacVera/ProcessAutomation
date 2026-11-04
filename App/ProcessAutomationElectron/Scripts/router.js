import { createRouter, createWebHashHistory } from 'vue-router';
import ProcessView from '../Pages/ProcessView.vue';
import TaskView from '../Pages/TaskView.vue';
import RunProcessView from '../Pages/RunProcessView.vue';

const routes = [
  {
    path: '/',
    redirect: '/Process'
  },
  {
    path: '/Process',
    name: 'Process',
    component: ProcessView
  },
  {
    path: '/Task',
    name: 'Task',
    component: TaskView,
    props: true
  },
  {
    path: '/RunProcess',
    name: 'RunProcess',
    component: RunProcessView
  }  
];

const router = createRouter({
  history: createWebHashHistory(),
  routes
});

export default router;