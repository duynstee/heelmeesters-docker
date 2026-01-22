import { createRouter, createWebHistory } from 'vue-router'
import LoginPage from '../views/LoginPage.vue'
import AgendaPage from '../views/HospitalPortal/AgendaPage.vue'
import CreateAppointmentPage from '../views/HospitalPortal/CreateAppointmentPage.vue'
import PatientPage from '../views/PatientPortal/PatientPage.vue'
import AdminHomePage from '../views/AdminPortal/AdminHomePage.vue'
import AdminCreateAccountPage from '../views/AdminPortal/AdminCreateAccountPage.vue'
import CreateReferralPage from '../views/GpPortal/CreateReferralPage.vue'

import { isLoggedIn, getRole } from '../services/authService'
import GpPage from '../views/GpPortal/GpPage.vue'

const routes = [
  { path: '/login', name: 'Login', component: LoginPage },

  {
    path: '/agenda',
    name: 'Agenda',
    component: AgendaPage,
    meta: { requiresAuth: true, role: 'Doctor' }
  },

  {
    path: '/appointment/new',
    name: 'NewAppointment',
    component: CreateAppointmentPage,
    meta: { requiresAuth: true, role: 'Doctor' }
  },

  {
    path: '/patient',
    name: 'PatientPortaal',
    component: PatientPage,
    meta: { requiresAuth: true, role: 'Patient' } // was 'User' → maak dit consistent met jouw backend
  },

  {
    path: '/admin',
    name: 'AdminHome',
    component: AdminHomePage,
    meta: { requiresAuth: true, role: 'Admin' }
  },
  
  {
  path: '/admin/create',
  name: 'AdminCreateAccount',
  component: AdminCreateAccountPage,
  meta: { requiresAuth: true, role: 'Admin' }
  },

  {
    path: '/gp',
    name: 'GpPage',
    component: GpPage,
    meta: { requiresAuth: true, role: 'GeneralPractitioner' }
  },

  {
    path: "/gp/referrals/new/:patientNumber",
    name: "CreateReferral",
    component: CreateReferralPage,
    meta: { requiresAuth: true, role: "GeneralPractitioner" }
  },

  { path: '/:pathMatch(.*)*', redirect: '/login' }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  if (to.meta.requiresAuth && !isLoggedIn()) return next('/login')

  // role guard
  const requiredRole = to.meta.role
  if (requiredRole) {
    const role = getRole()
    if (role !== requiredRole) {
      // simpele fallback redirects
      if (role === 'Admin') return next('/admin')
      if (role === 'Doctor') return next('/agenda')
      if (role === 'Patient') return next('/patient')
      if (role === 'GeneralPractitioner') return next('/gp')
      return next('/login')
    }
  }

  next()
})

export default router
