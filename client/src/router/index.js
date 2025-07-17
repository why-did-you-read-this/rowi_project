import { createRouter, createWebHistory } from 'vue-router'
import AgentView from '/src/views/AgentView.vue'
import BanksView from '/src/views/BanksView.vue'
import ClientsView from '/src/views/ClientsView.vue'

const routes = [
  { path: '/', redirect: '/agents' },
  { path: '/agents', component: AgentView },
  { path: '/banks', component: BanksView },
  { path: '/clients', component: ClientsView }
]

export default createRouter({
  history: createWebHistory(),
  routes
})
