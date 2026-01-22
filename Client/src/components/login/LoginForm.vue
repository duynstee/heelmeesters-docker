<template>
  <div class="login-card">
    <h2>Welkom terug</h2>
    <p class="subtitle">
      Log in om verder te gaan
    </p>

    <form @submit.prevent="submit">
      <div class="field">
        <label for="username">Gebruikersnaam</label>
        <input
          id="username"
          v-model="username"
          placeholder="Gebruikersnaam"
          required
        />
      </div>

      <div class="field">
        <label for="password">Wachtwoord</label>
        <input
          id="password"
          type="password"
          v-model="password"
          placeholder="Wachtwoord"
          required
        />
      </div>

      <button type="submit" class="submit">
        Inloggen
      </button>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const username = ref('')
const password = ref('')

const emit = defineEmits(['login'])

function submit() {
  emit('login', {
    username: username.value,
    password: password.value
  })
}
</script>

<style scoped>
.login-card {
  width: 100%;
  max-width: 420px;           
  padding: 36px 32px;
  border-radius: 14px;
  background: #ffffff;
  box-shadow: 0 12px 32px rgba(0, 0, 0, 0.12);
  margin: 0 auto;
}

h2 {
  text-align: center;
  margin-bottom: 6px;
  font-size: 22px;
  font-weight: 600;
}

.subtitle {
  text-align: center;
  color: #6b7280;
  font-size: 14px;
  margin-bottom: 28px;
}

.field {
  margin-bottom: 18px;
}

label {
  display: block;
  margin-bottom: 6px;
  font-size: 14px;
  font-weight: 500;
  color: #111827;
}

input {
  width: 100%;           
  height: 44px;
  padding: 0 14px;
  border-radius: 8px;
  border: 1px solid #d1d5db;
  font-size: 14px;
  background-color: #f9fafb;
  box-sizing: border-box;
}

input:focus {
  outline: none;
  border-color: #111827;
  background-color: #ffffff;
}

.submit {
  width: 100%;          
  height: 44px;
  margin-top: 8px;
  border-radius: 10px;
  border: none;
  background: linear-gradient(90deg, #020617, #111827);
  color: #ffffff;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
}

.submit:hover {
  opacity: 0.95;
}
</style>
