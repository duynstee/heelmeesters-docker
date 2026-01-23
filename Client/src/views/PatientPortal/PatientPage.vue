<template>
  <div class="min-h-screen bg-slate-50">
    <div class="mx-auto max-w-4xl px-6 py-8 space-y-6">
      <header class="space-y-1">
        <h1 class="text-2xl font-semibold">Patiënt portaal</h1>
        <p class="text-sm text-gray-600">
          Maak een afspraak op basis van een actieve doorverwijzing.
        </p>
      </header>

      <section class="grid gap-6">
        <div class="rounded-2xl border bg-white p-6 shadow-sm space-y-6">
          <h2 class="text-xl font-semibold">Afspraak maken</h2>

          <PatientReferralPicker
            :referrals="referrals"
            :loading="loadingReferrals"
            :error="referralsError"
            v-model:selectedReferral="selectedReferral"
            @refresh="loadReferrals"
          />

          <CreateAppointmentFromReferralForm
            :selectedReferral="selectedReferral"
            @created="onAppointmentCreated"
          />
        </div>

        <div class="rounded-2xl border bg-white p-6 shadow-sm space-y-4">
          <div class="flex items-center justify-between gap-2">
            <h2 class="text-xl font-semibold">Mijn afspraken</h2>
            <button
              class="rounded-xl border px-3 py-2 text-sm"
              type="button"
              @click="loadAppointments"
              :disabled="loadingAppointments"
            >
              {{ loadingAppointments ? "Laden..." : "Verversen" }}
            </button>
          </div>

          <MyAppointmentsList
            :appointments="appointments"
            :loading="loadingAppointments"
            :error="appointmentsError"
          />
        </div>

        <div class="rounded-2xl border bg-white p-6 shadow-sm">
          <MedischDosier />
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import {
  getMyActiveReferrals,
  getMyAppointments
} from "../../services/patientService";

import PatientReferralPicker from "../../components/PatientPortal/PatientReferralPicker.vue";
import CreateAppointmentFromReferralForm from "../../components/PatientPortal/CreateAppointmentFromReferralForm.vue";
import MyAppointmentsList from "../../components/PatientPortal/MyAppointmentsList.vue";
import MedischDosier from "../../components/PatientPortal/medischdosier.vue";

const referrals = ref([]);
const loadingReferrals = ref(false);
const referralsError = ref("");

const selectedReferral = ref(null);

const appointments = ref([]);
const loadingAppointments = ref(false);
const appointmentsError = ref("");

async function loadReferrals() {
  referralsError.value = "";
  loadingReferrals.value = true;

  try {
    referrals.value = await getMyActiveReferrals();

    // als geselecteerde referral niet meer bestaat (bv used), reset
    if (
      selectedReferral.value &&
      !referrals.value.some(r => r.id === selectedReferral.value.id)
    ) {
      selectedReferral.value = null;
    }
  } catch (e) {
    referralsError.value =
      e?.response?.data ?? e?.message ?? "Kon doorverwijzingen niet ophalen.";
  } finally {
    loadingReferrals.value = false;
  }
}

async function loadAppointments() {
  appointmentsError.value = "";
  loadingAppointments.value = true;

  try {
    appointments.value = await getMyAppointments();
  } catch (e) {
    appointmentsError.value =
      e?.response?.data ?? e?.message ?? "Kon afspraken niet ophalen.";
  } finally {
    loadingAppointments.value = false;
  }
}

async function onAppointmentCreated() {
  // na create: referrals opnieuw (de gebruikte verdwijnt), en afspraken opnieuw
  await loadReferrals();
  await loadAppointments();
}

onMounted(async () => {
  await loadReferrals();
  await loadAppointments();
});
</script>
