<script setup>
import { computed } from 'vue'
import { useRoute, useRouter, RouterLink } from 'vue-router'
import { logout, getRole, getEmail } from '../services/authService'

const router = useRouter()
const route = useRoute()

const role = computed(() => getRole())
const email = computed(() => getEmail())

function onLogout() {
  logout()
  router.push('/login')
}
</script>

<template>
  <nav class="navbar">
    <div class="left">
      <span class="welcome">Welkom {{ email || '' }}</span>
      <span v-if="role" style="margin-left:10px; color:#6b7280;">({{ role }})</span>
    </div>

    <div class="menu">
      <RouterLink
        v-if="role === 'Doctor'"
        to="/agenda"
        class="link"
        :class="{ active: route.path === '/agenda' }"
      >
        Agenda
      </RouterLink>

      <RouterLink
        v-if="role === 'Doctor'"
        to="/appointment/new"
        class="link"
        :class="{ active: route.path === '/appointment/new' }"
      >
        Nieuwe afspraak
      </RouterLink>

      <RouterLink
        v-if="role === 'Patient'"
        to="/patient"
        class="link"
        :class="{ active: route.path === '/patient' }"
      >
        Patiëntportaal
      </RouterLink>

      <RouterLink
        v-if="role === 'Admin'"
        to="/admin"
        class="link"
        :class="{ active: route.path === '/admin' }"
      >
        Admin
      </RouterLink>
    </div>

    <button class="logout" @click="onLogout">
      Logout
    </button>
  </nav>
</template>


<style scoped>
.navbar {
  height: 64px;
  padding: 0 32px;
  background: #ffffff;
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-bottom: 1px solid #e5e7eb;
}

.left {
  font-size: 14px;
  color: #111827;
}

.menu {
  display: flex;
  gap: 28px;
}

.link {
  text-decoration: none;
  font-size: 14px;
  color: #6b7280;
  position: relative;
}

.link.active {
  color: #111827;
  font-weight: 500;
}

.link.active::after {
  content: '';
  position: absolute;
  left: 0;
  right: 0;
  bottom: -22px;
  height: 2px;
  background: #111827;
}

.logout {
  background: none;
  border: none;
  font-size: 14px;
  cursor: pointer;
}
</style>
