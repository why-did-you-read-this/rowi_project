//import { createRouter, createWebHistory } from 'vue-router';

//// Пример маршрутов, добавьте свои компоненты и пути
//const routes = [
//  {
//    path: '/',
//    name: 'Home',
//    component: () => import('@/views/Home.vue'),
//  },
//  {
//    path: '/about',
//    name: 'About',
//    component: () => import('@/views/About.vue'),
//  },
//  // Добавьте другие маршруты по необходимости
//];

//const router = createRouter({
//  history: createWebHistory(),
//  routes,
//});

//export default router;

import { createRouter, createWebHistory } from 'vue-router'
import App from '../App.vue' // корневая
// сюда можно добавить страницы для банков и клиентов
const routes = [
  { path: '/', redirect: '/agents' },
  { path: '/agents', component: App }
]

export default createRouter({
  history: createWebHistory(),
  routes
})
