<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import {
  getDoctors,
  getPatients,
  getTreatments,
  getWeekSchedule,
  cancelAppointment as cancelAppointmentApi
} from '../../services/appointmentsService'

const weekStart = ref('')
const employeeNumber = ref('') // gekozen doctor
const doctors = ref([])

const appointments = ref([])
const loading = ref(false)
const loadingOptions = ref(false)

const errorMessage = ref('')
const successMessage = ref('')

// lookup maps
const patientNameByNumber = ref({})
const treatmentNameByCareCode = ref({})

function toIso(datetimeLocalString) {
  return new Date(datetimeLocalString).toISOString()
}

function formatDateTime(value) {
  try {
    return new Date(value).toLocaleString()
  } catch {
    return value
  }
}

function getDefaultWeekStart() {
  const now = new Date()
  const day = now.getDay() // 0=zo,1=ma
  const diffToMonday = (day === 0 ? -6 : 1 - day)
  const monday = new Date(now)
  monday.setDate(now.getDate() + diffToMonday)
  monday.setHours(0, 0, 0, 0)

  const pad = (n) => String(n).padStart(2, '0')
  const yyyy = monday.getFullYear()
  const mm = pad(monday.getMonth() + 1)
  const dd = pad(monday.getDate())
  const hh = pad(monday.getHours())
  const mi = pad(monday.getMinutes())
  return `${yyyy}-${mm}-${dd}T${hh}:${mi}`
}

const canLoad = computed(() => !!weekStart.value && !!employeeNumber.value)

const selectedDoctorName = computed(() => {
  const d = doctors.value.find(x => String(x.employeeNumber) === String(employeeNumber.value))
  return d?.name ?? ''
})

function patientLabel(patientNumber) {
  return patientNameByNumber.value?.[patientNumber] ?? `Patient ${patientNumber}`
}

function treatmentLabel(careCode) {
  return treatmentNameByCareCode.value?.[careCode] ?? careCode
}

async function loadOptions() {
  loadingOptions.value = true
  errorMessage.value = ''

  try {
    const [docData, patData, treatData] = await Promise.all([
      getDoctors(),
      getPatients(),
      getTreatments(),
    ])

    doctors.value = Array.isArray(docData) ? docData : []

    // auto-select eerste doctor
    if (!employeeNumber.value && doctors.value.length > 0) {
      employeeNumber.value = String(doctors.value[0].employeeNumber)
    }

    const patients = Array.isArray(patData) ? patData : []
    patientNameByNumber.value = Object.fromEntries(
      patients.map(p => [p.patientNumber, p.name])
    )

    const treatments = Array.isArray(treatData) ? treatData : []
    treatmentNameByCareCode.value = Object.fromEntries(
      treatments.map(t => [t.careCode, t.description])
    )
  } catch (err) {
    console.error(err)
    errorMessage.value = err?.response?.data?.message ?? 'Kon keuzelijsten niet ophalen.'
  } finally {
    loadingOptions.value = false
  }
}

async function loadWeekSchedule() {
  errorMessage.value = ''
  successMessage.value = ''
  if (!canLoad.value) return

  loading.value = true
  try {
    const data = await getWeekSchedule({
      weekStartIso: toIso(weekStart.value),
      employeeNumber: Number(employeeNumber.value)
    })
    appointments.value = Array.isArray(data) ? data : []
  } catch (err) {
    console.error(err)
    errorMessage.value = err?.response?.data?.message ?? 'Kon weekplanning niet ophalen.'
  } finally {
    loading.value = false
  }
}

async function cancelOne(id) {
  errorMessage.value = ''
  successMessage.value = ''

  const ok = confirm('Weet je zeker dat je deze afspraak wilt annuleren (verwijderen)?')
  if (!ok) return

  try {
    await cancelAppointmentApi(id)
    successMessage.value = 'Afspraak geannuleerd (verwijderd).'
    await loadWeekSchedule()
  } catch (err) {
    console.error(err)
    errorMessage.value = err?.response?.data?.message ?? 'Annuleren mislukt.'
  }
}

watch([weekStart, employeeNumber], async () => {
  if (canLoad.value) await loadWeekSchedule()
})

onMounted(async () => {
  weekStart.value = getDefaultWeekStart()
  await loadOptions()
  if (canLoad.value) await loadWeekSchedule()
})
</script>

<template>
  <div class="min-h-screen bg-slate-50">
    <div class="mx-auto max-w-6xl px-4 py-10">
      <div class="mb-6">
        <h1 class="text-2xl font-semibold text-slate-900">Agenda per dokter</h1>
        <p class="mt-1 text-sm text-slate-600">
          Kies een dokter en bekijk de afspraken van de geselecteerde week.
        </p>
      </div>

      <div v-if="errorMessage" class="mb-4 rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700">
        {{ errorMessage }}
      </div>

      <div v-if="successMessage" class="mb-4 rounded-lg border border-green-200 bg-green-50 p-3 text-sm text-green-700">
        {{ successMessage }}
      </div>

      <div class="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
        <div class="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
          <div class="flex flex-col gap-2">
            <label class="text-sm font-medium text-slate-800">Dokter</label>
            <select
              v-model="employeeNumber"
              class="w-full max-w-sm rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400"
              :disabled="loadingOptions"
            >
              <option value="" disabled>
                {{ loadingOptions ? 'Dokters laden…' : 'Selecteer dokter' }}
              </option>
              <option
                v-for="d in doctors"
                :key="d.employeeNumber"
                :value="String(d.employeeNumber)"
              >
                {{ d.name }} — {{ d.specialization }}
              </option>
            </select>
          </div>

          <div class="flex flex-col gap-2">
            <label class="text-sm font-medium text-slate-800">Week start</label>
            <input
              v-model="weekStart"
              type="datetime-local"
              class="w-full max-w-xs rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400"
            />
          </div>

          <div class="flex gap-2">
            <button
              class="rounded-lg border border-slate-200 px-4 py-2 text-sm text-slate-700 hover:bg-slate-50 disabled:opacity-50"
              @click="loadWeekSchedule"
              :disabled="loading || !employeeNumber || !weekStart"
            >
              {{ loading ? 'Laden…' : 'Herladen' }}
            </button>
          </div>
        </div>

        <!-- Dokternaam boven de tabel -->
        <div class="mt-6 flex items-center justify-between">
          <div class="text-sm text-slate-700">
            <span class="font-medium">Geselecteerde dokter:</span>
            <span class="ml-2">{{ selectedDoctorName || '—' }}</span>
          </div>
          <div v-if="loading" class="text-sm text-slate-500">Afspraken laden…</div>
        </div>

        <div class="mt-4 overflow-x-auto">
          <table class="min-w-full border-separate border-spacing-0">
            <thead>
              <tr class="text-left text-sm text-slate-600">
                <th class="border-b border-slate-200 px-3 py-2 font-medium">Patiënt</th>
                <th class="border-b border-slate-200 px-3 py-2 font-medium">Behandeling</th>
                <th class="border-b border-slate-200 px-3 py-2 font-medium">Kamer</th>
                <th class="border-b border-slate-200 px-3 py-2 font-medium">Start</th>
                <th class="border-b border-slate-200 px-3 py-2 font-medium">Eind</th>
                <th class="border-b border-slate-200 px-3 py-2 font-medium"></th>
              </tr>
            </thead>

            <tbody>
              <tr v-if="!loading && appointments.length === 0">
                <td colspan="6" class="px-3 py-6 text-center text-sm text-slate-500">
                  Geen afspraken gevonden voor deze week/dokter.
                </td>
              </tr>

              <tr v-for="a in appointments" :key="a.id" class="text-sm text-slate-800">
                <td class="border-b border-slate-100 px-3 py-2">
                  {{ patientLabel(a.patientNumber) }}
                </td>
                <td class="border-b border-slate-100 px-3 py-2">
                  {{ treatmentLabel(a.careCode) }}
                </td>
                <td class="border-b border-slate-100 px-3 py-2">
                  {{ a.roomCode }}
                </td>
                <td class="border-b border-slate-100 px-3 py-2">
                  {{ formatDateTime(a.startTime) }}
                </td>
                <td class="border-b border-slate-100 px-3 py-2">
                  {{ formatDateTime(a.endTime) }}
                </td>
                <td class="border-b border-slate-100 px-3 py-2 text-right">
                  <button
                    class="rounded-lg border border-red-200 bg-red-50 px-3 py-1.5 text-sm text-red-700 hover:bg-red-100"
                    @click="cancelOne(a.id)"
                  >
                    Cancel
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>
