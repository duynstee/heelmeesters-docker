<template>
  <AppNavbar v-if="$route.path !== '/login'" />
  
  <RouterView />

</template>

<script setup>
import { RouterLink, RouterView, useRouter } from "vue-router";
import { logout } from "../src/services/authService";
import AppNavbar from "./components/AppNavBar.vue";

const router = useRouter();

</script>
