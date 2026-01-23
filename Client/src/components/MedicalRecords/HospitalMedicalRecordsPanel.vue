<template>
  <section class="rounded-2xl border bg-white p-6 shadow-sm space-y-4">
    <header class="space-y-1">
      <h3 class="text-lg font-semibold">Medisch dossier</h3>
      <p class="text-sm text-gray-600">
        Overzicht en registratie van medische notities voor de geselecteerde patiënt.
      </p>
    </header>

    <p v-if="!patientNumber" class="text-sm text-gray-600">
      Selecteer eerst een patiënt om het dossier te zien.
    </p>

    <div v-else class="space-y-4">
      <div class="flex items-center justify-between gap-2">
        <div class="text-sm text-gray-600">
          Patiënt: <span class="font-medium text-slate-900">{{ patientLabel }}</span>
        </div>
        <button
          class="rounded-xl border px-3 py-2 text-sm"
          type="button"
          @click="loadRecords"
          :disabled="loading"
        >
          {{ loading ? "Laden..." : "Verversen" }}
        </button>
      </div>

      <div v-if="error" class="rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700">
        {{ error }}
      </div>

      <ul v-if="records.length" class="space-y-2">
        <li v-for="r in records" :key="r.lineNumber" class="rounded-xl border p-3">
          <div class="text-sm text-slate-700">{{ r.description || "Geen aanvullende notities" }}</div>
          <div class="mt-1 text-xs text-slate-500">
            Regel: <span class="font-mono">{{ r.lineNumber ?? "-" }}</span> •
            Datum: <span class="font-mono">{{ formatDate(r.createdOn) }}</span>
          </div>
          <div v-if="r.file" class="mt-3 flex flex-wrap gap-2">
            <button
              class="rounded-xl border border-blue-600 px-3 py-2 text-xs font-medium text-blue-700 shadow-sm hover:bg-blue-50"
              type="button"
              @click="viewFile(r)"
            >
              Bekijk PDF
            </button>
            <button
              class="rounded-xl bg-blue-600 px-3 py-2 text-xs font-medium text-white shadow-sm hover:bg-blue-700"
              type="button"
              @click="downloadFile(r)"
            >
              Download PDF
            </button>
          </div>
        </li>
      </ul>

      <p v-else class="text-sm text-gray-600">Geen dossierregels gevonden.</p>

      <hr />

      <div class="space-y-3">
        <h4 class="text-base font-semibold">Nieuwe dossierregel</h4>

        <div class="space-y-1">
          <label class="block text-sm font-medium">Omschrijving</label>
          <textarea
            v-model="description"
            class="w-full rounded-xl border px-3 py-2"
            rows="3"
            placeholder="Beschrijf de klacht/observatie/behandeling..."
            :disabled="saving"
          ></textarea>
        </div>

        <div class="space-y-2">
          <label class="block text-sm font-medium">Bestand (optioneel)</label>
          <input
            class="w-full rounded-xl border px-3 py-2"
            type="file"
            :disabled="saving"
            @change="onFileSelected"
          />
          <p v-if="fileName" class="text-xs text-slate-500">
            Gekozen bestand: <span class="font-medium">{{ fileName }}</span>
          </p>
          <p v-if="fileError" class="text-sm text-red-600">{{ fileError }}</p>
          <p v-else class="text-xs text-slate-500">
            Het bestand wordt als base64 meegestuurd.
          </p>
        </div>

        <div class="flex items-center gap-2">
          <button
            class="rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800 disabled:opacity-50"
            type="button"
            @click="createRecord"
            :disabled="saving || !canCreate"
          >
            {{ saving ? "Opslaan..." : "Opslaan" }}
          </button>
          <span v-if="success" class="text-sm text-green-700">{{ success }}</span>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed, ref, watch } from "vue";
import {
  createHospitalMedicalRecord,
  getHospitalMedicalRecordsByPatient
} from "../../services/medicalRecordsService";

const props = defineProps({
  patientNumber: { type: [String, Number], default: "" },
  patientName: { type: String, default: "" }
});

const records = ref([]);
const loading = ref(false);
const error = ref("");

const description = ref("");
const file = ref("");
const fileName = ref("");
const fileError = ref("");
const saving = ref(false);
const success = ref("");

const patientLabel = computed(() => {
  if (props.patientName) return `${props.patientName} (${props.patientNumber})`;
  return props.patientNumber ? `Patiënt ${props.patientNumber}` : "";
});

const canCreate = computed(() =>
  !!props.patientNumber && description.value.trim().length > 0
);

function formatDate(value) {
  if (!value) return "-";
  const d = new Date(value);
  return isNaN(d.getTime()) ? "-" : d.toLocaleString("nl-NL");
}

function buildPdfBlob(item) {
  if (!item?.file) return null;
  const data = item.file;

  let base64 = data;
  let mime = "application/pdf";

  if (typeof data === "string" && data.startsWith("data:")) {
    const [meta, payload] = data.split(",");
    base64 = payload ?? "";
    const match = meta.match(/data:([^;]+);base64/);
    if (match?.[1]) mime = match[1];
  }

  const byteCharacters = atob(base64);
  const byteNumbers = new Array(byteCharacters.length);
  for (let i = 0; i < byteCharacters.length; i++) {
    byteNumbers[i] = byteCharacters.charCodeAt(i);
  }
  const byteArray = new Uint8Array(byteNumbers);
  const blob = new Blob([byteArray], { type: mime || "application/pdf" });
  return { blob, mime: mime || "application/pdf" };
}

function viewFile(item) {
  try {
    const result = buildPdfBlob(item);
    if (!result) return;
    const url = URL.createObjectURL(result.blob);
    window.open(url, "_blank", "noopener,noreferrer");
    setTimeout(() => URL.revokeObjectURL(url), 1000);
  } catch {
    error.value = "PDF kon niet worden geopend.";
  }
}

function downloadFile(item) {
  try {
    const result = buildPdfBlob(item);
    if (!result) return;
    const { blob } = result;
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = `medical-record-${item.lineNumber ?? "file"}.pdf`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  } catch {
    error.value = "Bijlage kon niet worden gedownload.";
  }
}

async function loadRecords() {
  if (!props.patientNumber) return;
  loading.value = true;
  error.value = "";
  try {
    const data = await getHospitalMedicalRecordsByPatient(props.patientNumber);
    records.value = Array.isArray(data) ? data : [];
  } catch (e) {
    error.value = e?.response?.data ?? e?.message ?? "Kon dossier niet ophalen.";
  } finally {
    loading.value = false;
  }
}

async function createRecord() {
  if (!canCreate.value) return;
  saving.value = true;
  error.value = "";
  success.value = "";

  try {
    const payload = {
      patientNumber: Number(props.patientNumber),
      description: description.value.trim(),
      ...(file.value ? { file: file.value } : {})
    };

    await createHospitalMedicalRecord(payload);
    success.value = "Dossierregel opgeslagen.";
    description.value = "";
    file.value = "";
    fileName.value = "";
    await loadRecords();
  } catch (e) {
    error.value = e?.response?.data ?? e?.message ?? "Opslaan mislukt.";
  } finally {
    saving.value = false;
  }
}

function onFileSelected(event) {
  fileError.value = "";
  const input = event.target;
  const selected = input?.files?.[0];
  if (!selected) {
    file.value = "";
    fileName.value = "";
    return;
  }

  fileName.value = selected.name;

  const reader = new FileReader();
  reader.onload = () => {
    const result = reader.result;
    if (typeof result === "string") {
      // result is data URL; keep only base64 payload
      const commaIndex = result.indexOf(",");
      file.value = commaIndex >= 0 ? result.slice(commaIndex + 1) : result;
    }
  };
  reader.onerror = () => {
    fileError.value = "Bestand kon niet worden gelezen.";
    file.value = "";
    fileName.value = "";
  };
  reader.readAsDataURL(selected);
}

watch(
  () => props.patientNumber,
  async () => {
    records.value = [];
    error.value = "";
    success.value = "";
    if (props.patientNumber) await loadRecords();
  },
  { immediate: true }
);
</script>
