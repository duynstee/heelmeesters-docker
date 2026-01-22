<script setup>
import { ref, computed } from 'vue'
import {
  createPatientAccount,
  createGeneralPractitionerAccount,
  createHospitalStaffAccount
} from '../../services/adminService'
import { useRouter } from 'vue-router'

const router = useRouter()

const tab = ref('patient') // patient | gp | staff
const loading = ref(false)
const error = ref('')
const success = ref('')

// shared fields
const email = ref('')
const password = ref('')

// patient fields
const patientNumber = ref('')
const pFirstName = ref('')
const pPrefix = ref('')
const pLastName = ref('')
const pDob = ref('')

// gp fields
const gpId = ref('')
const gFirstName = ref('')
const gPrefix = ref('')
const gLastName = ref('')
const gDob = ref('')

// staff fields
const employeeNumber = ref('')
const sFirstName = ref('')
const sPrefix = ref('')
const sLastName = ref('')
const sDob = ref('')
const specialization = ref('')

const canSubmit = computed(() => {
  if (!email.value.trim() || !password.value.trim()) return false

  if (tab.value === 'patient') {
    return patientNumber.value && pFirstName.value.trim() && pLastName.value.trim() && pDob.value
  }
  if (tab.value === 'gp') {
    return gpId.value && gFirstName.value.trim() && gLastName.value.trim() && gDob.value
  }
  return employeeNumber.value && sFirstName.value.trim() && sLastName.value.trim() && sDob.value && specialization.value.trim()
})

function clearMessages() {
  error.value = ''
  success.value = ''
}

function resetForm() {
  email.value = ''
  password.value = ''

  patientNumber.value = ''
  pFirstName.value = ''
  pPrefix.value = ''
  pLastName.value = ''
  pDob.value = ''

  gpId.value = ''
  gFirstName.value = ''
  gPrefix.value = ''
  gLastName.value = ''
  gDob.value = ''

  employeeNumber.value = ''
  sFirstName.value = ''
  sPrefix.value = ''
  sLastName.value = ''
  sDob.value = ''
  specialization.value = ''
}

async function submit() {
  clearMessages()
  loading.value = true
  try {
    const base = {
      email: email.value.trim().toLowerCase(),
      password: password.value.trim()
    }

    if (tab.value === 'patient') {
      await createPatientAccount({
        ...base,
        patientNumber: Number(patientNumber.value),
        firstName: pFirstName.value.trim(),
        prefix: pPrefix.value.trim() || null,
        lastName: pLastName.value.trim(),
        dateOfBirth: pDob.value // yyyy-mm-dd
      })
      success.value = 'Patient account aangemaakt.'
    } else if (tab.value === 'gp') {
      await createGeneralPractitionerAccount({
        ...base,
        id: Number(gpId.value),
        firstName: gFirstName.value.trim(),
        prefix: gPrefix.value.trim() || null,
        lastName: gLastName.value.trim(),
        dateOfBirth: gDob.value
      })
      success.value = 'Huisarts account aangemaakt.'
    } else {
      await createHospitalStaffAccount({
        ...base,
        employeeNumber: Number(employeeNumber.value),
        firstName: sFirstName.value.trim(),
        prefix: sPrefix.value.trim() || null,
        lastName: sLastName.value.trim(),
        dateOfBirth: sDob.value,
        specialization: specialization.value.trim()
      })
      success.value = 'Specialist account aangemaakt.'
    }

    // na succes: leeg maken maar tab laten staan
    const currentTab = tab.value
    resetForm()
    tab.value = currentTab
  } catch (e) {
    console.error(e)
    error.value = e?.response?.data?.message ?? 'Aanmaken mislukt.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-slate-50">
    <div class="mx-auto max-w-3xl px-4 py-10">
      <div class="flex items-end justify-between">
        <div>
          <h1 class="text-2xl font-semibold text-slate-900">Account aanmaken</h1>
          <p class="mt-1 text-sm text-slate-600">Maak een patient, huisarts of specialist account.</p>
        </div>

        <button
          class="rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 hover:bg-slate-50"
          @click="router.push('/admin')"
        >
          Terug
        </button>
      </div>

      <div class="mt-6 flex flex-wrap gap-2">
        <button
          class="rounded-lg px-4 py-2 text-sm"
          :class="tab==='patient' ? 'bg-slate-900 text-white' : 'border border-slate-200 bg-white text-slate-700 hover:bg-slate-50'"
          @click="tab='patient'; clearMessages()"
        >
          Patient
        </button>
        <button
          class="rounded-lg px-4 py-2 text-sm"
          :class="tab==='gp' ? 'bg-slate-900 text-white' : 'border border-slate-200 bg-white text-slate-700 hover:bg-slate-50'"
          @click="tab='gp'; clearMessages()"
        >
          Huisarts
        </button>
        <button
          class="rounded-lg px-4 py-2 text-sm"
          :class="tab==='staff' ? 'bg-slate-900 text-white' : 'border border-slate-200 bg-white text-slate-700 hover:bg-slate-50'"
          @click="tab='staff'; clearMessages()"
        >
          Specialist
        </button>
      </div>

      <div v-if="error" class="mt-4 rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700">
        {{ error }}
      </div>
      <div v-if="success" class="mt-4 rounded-lg border border-green-200 bg-green-50 p-3 text-sm text-green-700">
        {{ success }}
      </div>

      <div class="mt-4 rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
        <!-- shared -->
        <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Email</label>
            <input v-model="email" type="email"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400"
              placeholder="bijv. user@dzh.nl"
            />
          </div>

          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Wachtwoord</label>
            <input v-model="password" type="text"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400"
              placeholder="test123"
            />
          </div>
        </div>

        <!-- PATIENT -->
        <div v-if="tab==='patient'" class="mt-6 grid grid-cols-1 gap-4 md:grid-cols-2">
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">PatientNumber</label>
            <input v-model="patientNumber" type="number"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>

          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Geboortedatum</label>
            <input v-model="pDob" type="date"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>

          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Voornaam</label>
            <input v-model="pFirstName" type="text"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>

          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Tussenvoegsel (optioneel)</label>
            <input v-model="pPrefix" type="text"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>

          <div class="md:col-span-2">
            <label class="mb-1 block text-sm font-medium text-slate-800">Achternaam</label>
            <input v-model="pLastName" type="text"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>
        </div>

        <!-- GP -->
        <div v-else-if="tab==='gp'" class="mt-6 grid grid-cols-1 gap-4 md:grid-cols-2">
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Huisarts Id</label>
            <input v-model="gpId" type="number"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>

          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Geboortedatum</label>
            <input v-model="gDob" type="date"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>

          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Voornaam</label>
            <input v-model="gFirstName" type="text"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>

          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Tussenvoegsel (optioneel)</label>
            <input v-model="gPrefix" type="text"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>

          <div class="md:col-span-2">
            <label class="mb-1 block text-sm font-medium text-slate-800">Achternaam</label>
            <input v-model="gLastName" type="text"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>
        </div>

        <!-- STAFF -->
        <div v-else class="mt-6 grid grid-cols-1 gap-4 md:grid-cols-2">
          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">EmployeeNumber</label>
            <input v-model="employeeNumber" type="number"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>

          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Specialisatie</label>
            <input v-model="specialization" type="text"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400"
              placeholder="Bijv. Cardiologie" />
          </div>

          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Geboortedatum</label>
            <input v-model="sDob" type="date"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>

          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Voornaam</label>
            <input v-model="sFirstName" type="text"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>

          <div>
            <label class="mb-1 block text-sm font-medium text-slate-800">Tussenvoegsel (optioneel)</label>
            <input v-model="sPrefix" type="text"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>

          <div class="md:col-span-2">
            <label class="mb-1 block text-sm font-medium text-slate-800">Achternaam</label>
            <input v-model="sLastName" type="text"
              class="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm outline-none focus:border-slate-400" />
          </div>
        </div>

        <div class="mt-6 flex items-center justify-end gap-3">
          <button
            class="rounded-lg border border-slate-200 px-4 py-2 text-sm text-slate-700 hover:bg-slate-50"
            type="button"
            @click="resetForm"
          >
            Reset
          </button>

          <button
            class="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800 disabled:opacity-50"
            type="button"
            :disabled="!canSubmit || loading"
            @click="submit"
          >
            {{ loading ? 'Aanmaken…' : 'Maak account' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>