<template>
  <AppNavBar v-if="$route.path !== '/login'" />

  <RouterView />
</template>

<script setup>
import { RouterLink, RouterView, useRouter } from "vue-router";
import { logout } from "../src/services/authService";
import AppNavBar from "./components/AppNavbar.vue";

const router = useRouter();
</script>
