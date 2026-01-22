<script setup>
import { ref, computed, onMounted } from 'vue'
import {
  getDoctors,
  getPatients,
  getRooms,
  getTreatments,
  createAppointment as createAppointmentApi
} from '../../services/appointmentsService'


const loading = ref(false)
const submitting = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const doctors = ref([])
const patients = ref([])
const rooms = ref([])
const treatments = ref([])

const doctorSearch = ref('')
const patientSearch = ref('')

const employeeNumber = ref('')
const patientNumber = ref('')
const careCode = ref('')
const roomCode = ref('')
const startTime = ref('')
const endTime = ref('') // optioneel

const canSubmit = computed(() =>
  employeeNumber.value &&
  patientNumber.value &&
  careCode.value &&
  roomCode.value &&
  startTime.value
)

function toIso(datetimeLocalString) {
  return new Date(datetimeLocalString).toISOString()
}

const filteredDoctors = computed(() => {
  const q = doctorSearch.value.trim().toLowerCase()
  if (!q) return doctors.value
  return doctors.value.filter(d =>
    String(d.employeeNumber).includes(q) ||
    (d.name ?? '').toLowerCase().includes(q) ||
    (d.specialization ?? '').toLowerCase().includes(q)
  )
})

const filteredPatients = computed(() => {
  const q = patientSearch.value.trim().toLowerCase()
  if (!q) return patients.value
  return patients.value.filter(p =>
    String(p.patientNumber).includes(q) ||
    (p.name ?? '').toLowerCase().includes(q)
  )
})

async function loadOptions() {
  loading.value = true
  errorMessage.value = ''
  try {
    const [docData, patData, roomData, treatData] = await Promise.all([
      getDoctors(),
      getPatients(),
      getRooms(),
      getTreatments(),
    ])

    doctors.value = Array.isArray(docData) ? docData : []
    patients.value = Array.isArray(patData) ? patData : []
    rooms.value = Array.isArray(roomData) ? roomData : []
    treatments.value = Array.isArray(treatData) ? treatData : []
  } catch (err) {
    console.error(err)
    errorMessage.value = err?.response?.data?.message ?? 'Kon keuzelijsten niet ophalen.'
  } finally {
    loading.value = false
  }
}


async function createAppointment() {
  submitting.value = true
  errorMessage.value = ''
  successMessage.value = ''

  try {
    const payload = {
      employeeNumber: Number(employeeNumber.value),
      patientNumber: Number(patientNumber.value),
      careCode: careCode.value,
      roomCode: roomCode.value,
      startTime: toIso(startTime.value),
      ...(endTime.value ? { endTime: toIso(endTime.value) } : {})
    }

    await createAppointmentApi(payload)

    successMessage.value = 'Afspraak succesvol aangemaakt!'

    employeeNumber.value = ''
    patientNumber.value = ''
    careCode.value = ''
    roomCode.value = ''
    startTime.value = ''
    endTime.value = ''
    doctorSearch.value = ''
    patientSearch.value = ''
  } catch (err) {
    console.error(err)
    errorMessage.value = err?.response?.data?.message ?? 'Fout bij aanmaken afspraak.'
  } finally {
    submitting.value = false
  }
}

onMounted(loadOptions)
</script>

<template>
  <div class="min-h-screen bg-slate-50">
    <div class="mx-auto max-w-3xl px-4 py-10">
      <div class="mb-6">
        <h1 class="text-2xl font-semibold text-slate-900">Nieuwe afspraak</h1>
        <p class="mt-1 text-sm text-slate-600">
          Kies dokter, patiënt, behandeling en kamer. Daarna plan je het tijdstip.
        </p>
      </div>

      <div v-if="errorMessage" class="mb-4 rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700">
        {{ errorMessage }}
      </div>

      <div v-if="successMessage" class="mb-4 rounded-lg border border-green-200 bg-green-50 p-3 text-sm text-green-700">
        {{ successMessage }}
      </div>

      <div class="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
        <div class="mb-4 flex items-center justify-between">
          <div class="text-sm text-slate-600">
            <span v-if="loading">Lijstjes laden…</span>
            <span v-else>Keuzelijsten geladen.</span>
          </div>
          <button
            class="rounded-lg border border-slate-200 px-3 py-1.5 text-sm text-slate-700 hover:bg-slate-50"
            @click="loadOptions"
            :disabled="loading"
          >
            Herladen
          </button>
        </div>

        <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
          <!-- Doctor -->
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Dokter</label>
            <input
              v-model="doctorSearch"
              type="text"
              placeholder="Zoek op naam of nummer…"
              class="mb-2 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400"
            />
            <select
              v-model="employeeNumber"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400"
              :disabled="loading"
            >
              <option value="" disabled>Selecteer dokter</option>
              <option v-for="d in filteredDoctors" :key="d.employeeNumber" :value="d.employeeNumber">
                {{ d.name }} — {{ d.specialization }} ({{ d.employeeNumber }})
              </option>
            </select>
          </div>

          <!-- Patient -->
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Patiënt</label>
            <input
              v-model="patientSearch"
              type="text"
              placeholder="Zoek op naam of nummer…"
              class="mb-2 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400"
            />
            <select
              v-model="patientNumber"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400"
              :disabled="loading"
            >
              <option value="" disabled>Selecteer patiënt</option>
              <option v-for="p in filteredPatients" :key="p.patientNumber" :value="p.patientNumber">
                {{ p.name }} ({{ p.patientNumber }})
              </option>
            </select>
          </div>

          <!-- Treatment -->
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Behandeling</label>
            <select
              v-model="careCode"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400"
              :disabled="loading"
            >
              <option value="" disabled>Selecteer behandeling</option>
              <option v-for="t in treatments" :key="t.careCode" :value="t.careCode">
                {{ t.careCode }} — {{ t.description }}
              </option>
            </select>
          </div>

          <!-- Room -->
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Kamer</label>
            <select
              v-model="roomCode"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400"
              :disabled="loading"
            >
              <option value="" disabled>Selecteer kamer</option>
              <option v-for="r in rooms" :key="r.roomCode" :value="r.roomCode">
                {{ r.roomCode }}
              </option>
            </select>
          </div>

          <!-- Start -->
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Start</label>
            <input
              v-model="startTime"
              type="datetime-local"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400"
            />
          </div>

          <!-- End -->
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">
              Eind (optioneel)
              <span class="ml-1 text-xs text-slate-500">(leeg = +30 min)</span>
            </label>
            <input
              v-model="endTime"
              type="datetime-local"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400"
            />
          </div>
        </div>

        <div class="mt-6 flex items-center justify-end gap-3">
          <button
            class="rounded-lg border border-slate-200 px-4 py-2 text-sm text-slate-700 hover:bg-slate-50"
            @click="() => { employeeNumber=''; patientNumber=''; careCode=''; roomCode=''; startTime=''; endTime=''; }"
            type="button"
          >
            Reset
          </button>

          <button
            class="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800 disabled:opacity-50"
            @click="createAppointment"
            :disabled="!canSubmit || submitting"
            type="button"
          >
            {{ submitting ? 'Aanmaken…' : 'Maak afspraak' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
