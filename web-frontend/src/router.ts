import { createRouter, createWebHistory } from 'vue-router'
import ClassList from './views/ClassList.vue'
import ClassOverview from './views/ClassOverview.vue'
import Leaderboard from './views/Leaderboard.vue'

const routes = [
  { path: '/', name: 'ClassList', component: ClassList },
  { path: '/class/:id', name: 'ClassOverview', component: ClassOverview, props: true },
  { path: '/leaderboard', name: 'Leaderboard', component: Leaderboard },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
