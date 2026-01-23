<template>
  <section class="rounded-2xl border bg-white p-6 shadow-sm space-y-6">
    <header>
      <h2 class="text-xl font-semibold">Patiënten</h2>
      <p class="text-sm text-gray-600">
        Zoek en selecteer een patiënt om de actieve doorverwijzingen te zien.
      </p>
    </header>

    <!-- Zoek + dropdown -->
    <div class="space-y-4">
      <div class="space-y-1">
        <label class="block text-sm font-medium">Zoek patiënt</label>
        <input
          type="text"
          v-model="patientSearch"
          placeholder="Zoek op naam of patiëntnummer"
          class="w-full rounded-xl border px-3 py-2"
          :disabled="loadingPatients"
        />
      </div>

      <div class="space-y-1">
        <label class="block text-sm font-medium">Patiënt</label>

        <div class="flex gap-2">
          <select
            class="w-full rounded-xl border px-3 py-2"
            v-model="selectedPatientNumber"
            :disabled="loadingPatients"
          >
            <option value="" disabled>
              {{ loadingPatients ? "Patiënten laden..." : "Kies een patiënt" }}
            </option>

            <option
              v-for="p in filteredPatients"
              :key="p.patientNumber"
              :value="String(p.patientNumber)"
            >
              {{ patientLabel(p) }}
            </option>
          </select>

          <button
            class="rounded-xl border px-3 py-2"
            type="button"
            @click="reloadPatients"
            :disabled="loadingPatients"
          >
            Vernieuwen
          </button>
        </div>

        <p v-if="patientsError" class="text-sm text-red-600">{{ patientsError }}</p>

        <p
          v-if="!loadingPatients && !patientsError && filteredPatients.length === 0"
          class="text-sm text-gray-500"
        >
          Geen patiënten gevonden.
        </p>
      </div>
    </div>

    <hr />

    <!-- Referrals -->
    <div class="space-y-3">
      <div class="flex items-center justify-between gap-2">
        <h3 class="text-lg font-semibold">Actieve doorverwijzingen</h3>

        <div class="flex items-center gap-2">
          <button
            v-if="selectedPatientNumber"
            class="rounded-xl border px-3 py-2 text-sm"
            type="button"
            @click="goToCreateReferral"
          >
            + Nieuwe doorverwijzing
          </button>

          <button
            v-if="selectedPatientNumber"
            class="rounded-xl border px-3 py-2 text-sm"
            type="button"
            @click="refreshReferrals"
            :disabled="loadingReferrals"
          >
            {{ loadingReferrals ? "Verversen..." : "Verversen" }}
          </button>
        </div>
      </div>

      <p v-if="!selectedPatientNumber" class="text-sm text-gray-600">
        Selecteer eerst een patiënt.
      </p>

      <div v-else>
        <p v-if="loadingReferrals" class="text-sm text-gray-600">
          Doorverwijzingen laden...
        </p>

        <p v-if="referralsError" class="text-sm text-red-600">
          {{ referralsError }}
        </p>

        <p
          v-if="!loadingReferrals && !referralsError && referrals.length === 0"
          class="text-sm text-gray-600"
        >
          Geen actieve doorverwijzingen gevonden.
        </p>

        <ul v-if="referrals.length > 0" class="space-y-2">
          <li v-for="r in referrals" :key="r.id" class="rounded-xl border p-3">
            <div class="flex items-center justify-between gap-3">
              <div>
                <div class="font-medium">Doorverwijzing</div>

                <div class="mt-1 space-y-1">
                  <div class="text-sm text-gray-600">
                    Behandeling:
                    <span class="font-medium">{{ r.treatmentDescription }}</span>
                  </div>

                  <div class="text-xs text-gray-500">
                    Care code:
                    <span class="font-mono">{{ r.careCode }}</span>
                  </div>
                  <div class="text-xs text-gray-500">
                    Gebruikt: <span class="font-medium">{{ r.isUsed ? "Ja" : "Nee" }}</span>
                    </div>
                        <div v-if="r.usedOn" class="text-xs text-gray-500">
                            Gebruikt op: <span class="font-mono">{{ new Date(r.usedOn).toLocaleString() }}</span>
                        </div>
                    </div>
              </div>

              <span class="rounded-full bg-green-50 px-3 py-1 text-xs text-green-700">
                Actief
              </span>
            </div>
          </li>
        </ul>
      </div>
    </div>

    <hr />

    <HospitalMedicalRecordsPanel
      :patientNumber="selectedPatientNumber"
      :patientName="selectedPatient?.fullName ?? ''"
    />
  </section>
</template>

<script setup>
import { computed, onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { getMyPatients, getReferralsForPatient } from "../../services/gpService";
import HospitalMedicalRecordsPanel from "../MedicalRecords/HospitalMedicalRecordsPanel.vue";

const router = useRouter();
const route = useRoute();

const patients = ref([]);
const loadingPatients = ref(false);
const patientsError = ref("");

const patientSearch = ref("");
const selectedPatientNumber = ref("");

const referrals = ref([]);
const loadingReferrals = ref(false);
const referralsError = ref("");

function normalizePatient(p) {
  return {
    ...p,
    patientNumber:
      p.patientNumber ?? p.PatientNumber ?? p.patient_number ?? p.patientnr,
    firstName: p.firstName ?? p.FirstName ?? p.first_name,
    prefix: p.prefix ?? p.Prefix ?? p.prefixName ?? p.tussenvoegsel,
    lastName: p.lastName ?? p.LastName ?? p.last_name,
    name: p.name ?? p.Name
  };
}

function patientLabel(p) {
  const fullName = [p.firstName, p.prefix, p.lastName].filter(Boolean).join(" ");
  const label = fullName || p.name || "Patiënt";
  return `${label} (${p.patientNumber})`;
}

const selectedPatient = computed(() => {
  if (!selectedPatientNumber.value) return null;
  const p = patients.value.find(
    x => String(x.patientNumber) === String(selectedPatientNumber.value)
  );
  if (!p) return null;
  const fullName = [p.firstName, p.prefix, p.lastName].filter(Boolean).join(" ");
  return { ...p, fullName: fullName || p.name || "" };
});

const filteredPatients = computed(() => {
  const q = patientSearch.value.trim().toLowerCase();
  if (!q) return patients.value;

  return patients.value.filter((p) => {
    const fullName = [p.firstName, p.prefix, p.lastName]
      .filter(Boolean)
      .join(" ")
      .toLowerCase();

    const name = (p.name ?? "").toLowerCase();
    return (
      fullName.includes(q) ||
      name.includes(q) ||
      String(p.patientNumber).includes(q)
    );
  });
});

function applyPreselectFromQuery() {
  const pn = route.query.patientNumber;
  if (!pn) return;

  const exists = patients.value.some(
    p => String(p.patientNumber) === String(pn)
  );
  if (!exists) return;

  selectedPatientNumber.value = String(pn);

  // query opruimen zodat het niet blijft hangen
  const q = { ...route.query };
  delete q.patientNumber;
  router.replace({ path: "/gp", query: q });
}

async function loadPatients() {
  patientsError.value = "";
  loadingPatients.value = true;

  try {
    const data = await getMyPatients({ take: 200 });
    patients.value = Array.isArray(data) ? data.map(normalizePatient) : [];
    console.log("GP patients loaded:", patients.value);
    applyPreselectFromQuery();
  } catch (e) {
    patientsError.value =
      e?.response?.data ?? e?.message ?? "Kon patiënten niet ophalen.";
  } finally {
    loadingPatients.value = false;
  }
}

async function loadReferrals(patientNumber) {
  referralsError.value = "";
  loadingReferrals.value = true;
  referrals.value = [];

  try {
    referrals.value = await getReferralsForPatient(patientNumber);
  } catch (e) {
    referralsError.value =
      e?.response?.data ?? e?.message ?? "Kon doorverwijzingen niet ophalen.";
  } finally {
    loadingReferrals.value = false;
  }
}

async function reloadPatients() {
  patientSearch.value = "";
  selectedPatientNumber.value = "";
  referrals.value = [];
  await loadPatients();
}

async function refreshReferrals() {
  if (!selectedPatientNumber.value) return;
  await loadReferrals(selectedPatientNumber.value);
}

function goToCreateReferral() {
  if (!selectedPatientNumber.value) return;

  const patient = patients.value.find(
    p => String(p.patientNumber) === String(selectedPatientNumber.value)
  );

  const patientName = patient
    ? [patient.firstName, patient.prefix, patient.lastName].filter(Boolean).join(" ")
    : "";

  router.push({
    path: `/gp/referrals/new/${selectedPatientNumber.value}`,
    query: { patientName }
  });
}

onMounted(loadPatients);

watch(patientSearch, () => {
  selectedPatientNumber.value = "";
  referrals.value = [];
  referralsError.value = "";
});

watch(selectedPatientNumber, async (newVal) => {
  referrals.value = [];
  referralsError.value = "";
  if (!newVal) return;
  await loadReferrals(newVal);
});
</script>
