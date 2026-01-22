<template>
  <div class="min-h-screen bg-slate-50">
    <div class="mx-auto max-w-3xl px-6 py-8 space-y-6">
      <header class="space-y-1">
        <h1 class="text-2xl font-semibold">Nieuwe doorverwijzing</h1>
        <p class="text-sm text-gray-600">
          Maak een nieuwe doorverwijzing voor patiënt {{ patientNumber }}.
        </p>
      </header>

      <CreateReferralForm
        :patient-number="patientNumber"
        :patient-name="patientName"
        @created="onCreated"
      />

      <div>
        <button
          class="text-sm text-blue-600 hover:underline"
          @click="goBack"
        >
          ← Terug naar overzicht
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRoute, useRouter } from "vue-router";
import CreateReferralForm from "../../components/GpPortal/CreateReferralForm.vue";

const route = useRoute();
const router = useRouter();

const patientNumber = route.params.patientNumber;

function onCreated() {
  router.push({
    path: "/gp",
    query: { patientNumber: String(patientNumber) }
  });
}

function goBack() {
  router.back();
}
</script>
