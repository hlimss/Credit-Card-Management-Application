import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'Dashboard',
      component: () => import('../views/Dashboard.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/cards',
      name: 'Home',
      component: () => import('../views/Home.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/login',
      name: 'Login',
      component: () => import('../views/Login.vue'),
      meta: { requiresGuest: true }
    },
    {
      path: '/register',
      name: 'Register',
      component: () => import('../views/Register.vue'),
      meta: { requiresGuest: true }
    },
    {
      path: '/oauth-callback',
      name: 'OAuthCallback',
      component: () => import('../views/OAuthCallback.vue')
    },
    {
      path: '/statements',
      name: 'Statements',
      component: () => import('../components/StatementView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/loans',
      name: 'Loans',
      component: () => import('../components/LoanView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/analytics',
      name: 'Analytics',
      component: () => import('../components/AnalyticsView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/transactions',
      name: 'Transactions',
      component: () => import('../views/Transactions.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/settings',
      name: 'Settings',
      component: () => import('../views/Settings.vue'),
      meta: { requiresAuth: true }
    }
  ]
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({ name: 'Login' })
  } else if (to.meta.requiresGuest && authStore.isAuthenticated) {
    next({ name: 'Home' })
  } else {
    next()
  }
})

export default router

