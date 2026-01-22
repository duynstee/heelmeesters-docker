<template>
  <div class="login-page">
    <LoginForm @login="handleLogin" />

    <p v-if="errorMessage" class="error">
      {{ errorMessage }}
    </p>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { login, getRole } from '../services/authService'
import LoginForm from '../components/login/LoginForm.vue'

const errorMessage = ref('')
const router = useRouter()

const handleLogin = async ({ username, password }) => {
  errorMessage.value = ''
  try {
    await login(username, password)
    const role = getRole()

    if (role === 'Admin') return router.push('/admin')
    if (role === 'Doctor') return router.push('/agenda')
    return router.push('/patient')
  } catch (err) {
    errorMessage.value = 'Inloggen mislukt.'
  }
}

</script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background: #f9fafb;
}

.error {
  margin-top: 16px;
  color: #dc2626;
}
</style>
