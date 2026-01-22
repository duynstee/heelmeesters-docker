<template>
  <section class="rounded-2xl border bg-white p-6 shadow-sm space-y-4">
    <header class="space-y-1">
      <h2 class="text-xl font-semibold">Nieuwe doorverwijzing</h2>

      <p class="text-sm text-gray-600" v-if="patientNumber">
        Voor patiënt: <span class="font-medium">{{ patientName }}</span>
      </p>

      <p class="text-sm text-gray-600" v-else>
        Selecteer eerst een patiënt.
      </p>
    </header>

    <div v-if="patientNumber" class="space-y-2">
      <label class="block text-sm font-medium">Behandeling</label>

      <div class="flex gap-2">
        <select
          class="w-full rounded-xl border px-3 py-2"
          v-model="selectedCareCode"
          :disabled="loadingTreatments || saving"
        >
          <option value="" disabled>
            {{ loadingTreatments ? "Treatments laden..." : "Kies een treatment" }}
          </option>

          <option v-for="t in treatments" :key="t.careCode" :value="t.careCode">
            {{ t.careCode }} — {{ t.description }}
          </option>
        </select>

        <button
          class="rounded-xl border px-3 py-2"
          type="button"
          @click="reloadTreatments"
          :disabled="loadingTreatments || saving"
        >
          Vernieuwen
        </button>
      </div>

      <p v-if="treatmentsError" class="text-sm text-red-600">
        {{ treatmentsError }}
      </p>

      <div class="pt-2 flex gap-2 items-center">
        <button
          class="rounded-xl border px-4 py-2"
          type="button"
          @click="submit"
          :disabled="saving || !canSubmit"
        >
          {{ saving ? "Aanmaken..." : "Aanmaken" }}
        </button>

        <span v-if="success" class="text-sm text-green-700">
          {{ success }}
        </span>
      </div>

      <p v-if="error" class="text-sm text-red-600">{{ error }}</p>
    </div>
  </section>
</template>

<script setup>
import { computed, onMounted, ref, watch } from "vue";
import { createReferral, getGpTreatments } from "../../services/gpService";

const props = defineProps({
  patientNumber: { type: [String, Number], required: true },
  patientName: { type: String, default: "" }
});

const emit = defineEmits(["created"]);

const treatments = ref([]);
const loadingTreatments = ref(false);
const treatmentsError = ref("");

const selectedCareCode = ref("");

const saving = ref(false);
const error = ref("");
const success = ref("");

const canSubmit = computed(() => {
  return !!String(props.patientNumber || "").trim() && !!selectedCareCode.value;
});

async function loadTreatments() {
  treatmentsError.value = "";
  loadingTreatments.value = true;

  try {
    treatments.value = await getGpTreatments();
  } catch (e) {
    treatmentsError.value =
      e?.response?.data ?? e?.message ?? "Kon treatments niet ophalen.";
  } finally {
    loadingTreatments.value = false;
  }
}

async function reloadTreatments() {
  selectedCareCode.value = "";
  await loadTreatments();
}

async function submit() {
  error.value = "";
  success.value = "";

  const pn = String(props.patientNumber || "").trim();
  if (!pn) {
    error.value = "Geen patiënt geselecteerd.";
    return;
  }
  if (!selectedCareCode.value) {
    error.value = "Selecteer een behandeling.";
    return;
  }

  saving.value = true;
  try {
    await createReferral(Number(pn), selectedCareCode.value);

    success.value = "Doorverwijzing aangemaakt.";
    selectedCareCode.value = "";

    emit("created");
  } catch (e) {
    error.value = e?.response?.data ?? e?.message ?? "Aanmaken mislukt.";
  } finally {
    saving.value = false;
  }
}

onMounted(loadTreatments);

watch(
  () => props.patientNumber,
  () => {
    selectedCareCode.value = "";
    error.value = "";
    success.value = "";
  }
);
</script>
