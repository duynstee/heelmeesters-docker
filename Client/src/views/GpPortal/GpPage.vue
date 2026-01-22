<template>
  <div class="min-h-screen bg-slate-50">
    <div class="mx-auto max-w-5xl px-6 py-8 space-y-6">
      <h1 class="text-2xl font-semibold">Huisartsportaal</h1>

      <PatientReferralPanel />
    </div>
  </div>
</template>

<script setup>
import PatientReferralPanel from "../../components/GpPortal/PatientReferralPanel.vue";
</script>
