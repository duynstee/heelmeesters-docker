<script setup>
import { ref, computed, onMounted } from 'vue'
import { getPatients, getGeneralPractitioners, getHospitalStaff } from '../../services/adminService'

const tab = ref('patients') // patients | gps | staff
const loading = ref(false)
const error = ref('')

const patients = ref([])
const gps = ref([])
const staff = ref([])

const q = ref('')

function formatDate(d) {
  try {
    return new Date(d).toLocaleDateString()
  } catch {
    return d
  }
}

const title = computed(() => {
  if (tab.value === 'patients') return 'Patiënten'
  if (tab.value === 'gps') return 'Huisartsen'
  return 'Specialisten'
})

const filtered = computed(() => {
  const query = q.value.trim().toLowerCase()
  let list = []
  if (tab.value === 'patients') list = patients.value
  if (tab.value === 'gps') list = gps.value
  if (tab.value === 'staff') list = staff.value

  if (!query) return list

  return list.filter(x => {
    const hay = [
      x.name,
      x.email,
      x.patientNumber,
      x.employeeNumber,
      x.id,
      x.specialization
    ]
      .filter(Boolean)
      .join(' ')
      .toLowerCase()

    return hay.includes(query)
  })
})

async function load() {
  loading.value = true
  error.value = ''
  try {
    const [p, g, s] = await Promise.all([
      getPatients(),
      getGeneralPractitioners(),
      getHospitalStaff()
    ])

    patients.value = Array.isArray(p) ? p : []
    gps.value = Array.isArray(g) ? g : []
    staff.value = Array.isArray(s) ? s : []
  } catch (e) {
    console.error(e)
    error.value = e?.response?.data?.message ?? 'Kon admin overzicht niet laden.'
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="min-h-screen bg-slate-50">
    <div class="mx-auto max-w-6xl px-4 py-10">
      <div class="flex flex-col gap-2 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <h1 class="text-2xl font-semibold text-slate-900">Admin portaal</h1>
          <p class="mt-1 text-sm text-slate-600">Overzicht van accounts in het systeem.</p>
        </div>

        <button
          class="rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 hover:bg-slate-50 disabled:opacity-50"
          :disabled="loading"
          @click="load"
        >
          {{ loading ? 'Laden…' : 'Ververs' }}
        </button>
        <button
          class="rounded-lg bg-slate-900 px-3 py-2 text-sm text-white hover:bg-slate-800"
          @click="$router.push('/admin/create')"
        >
          + Account
        </button>
      </div>

      <div class="mt-6 flex flex-wrap gap-2">
        <button
          class="rounded-lg px-4 py-2 text-sm"
          :class="tab === 'patients' ? 'bg-slate-900 text-white' : 'border border-slate-200 bg-white text-slate-700 hover:bg-slate-50'"
          @click="tab='patients'; q=''"
        >
          Patiënten ({{ patients.length }})
        </button>

        <button
          class="rounded-lg px-4 py-2 text-sm"
          :class="tab === 'gps' ? 'bg-slate-900 text-white' : 'border border-slate-200 bg-white text-slate-700 hover:bg-slate-50'"
          @click="tab='gps'; q=''"
        >
          Huisartsen ({{ gps.length }})
        </button>

        <button
          class="rounded-lg px-4 py-2 text-sm"
          :class="tab === 'staff' ? 'bg-slate-900 text-white' : 'border border-slate-200 bg-white text-slate-700 hover:bg-slate-50'"
          @click="tab='staff'; q=''"
        >
          Specialisten ({{ staff.length }})
        </button>
      </div>

      <div class="mt-4 flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
        <h2 class="text-lg font-semibold text-slate-900">{{ title }}</h2>

        <input
          v-model="q"
          type="text"
          placeholder="Zoeken op naam, email, nummer, specialisatie…"
          class="w-full rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm outline-none focus:border-slate-400 sm:max-w-md"
        />
      </div>

      <div v-if="error" class="mt-4 rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700">
        {{ error }}
      </div>

      <div class="mt-4 rounded-xl border border-slate-200 bg-white shadow-sm">
        <div class="overflow-x-auto">
          <!-- Patients table -->
          <table v-if="tab==='patients'" class="w-full text-left text-sm">
            <thead class="border-b border-slate-200 bg-slate-50 text-slate-700">
              <tr>
                <th class="px-4 py-3">Naam</th>
                <th class="px-4 py-3">Email</th>
                <th class="px-4 py-3">PatientNumber</th>
                <th class="px-4 py-3">Geboortedatum</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="!loading && filtered.length===0">
                <td class="px-4 py-4 text-slate-600" colspan="4">Geen resultaten.</td>
              </tr>
              <tr v-for="p in filtered" :key="p.patientNumber" class="border-b border-slate-100 hover:bg-slate-50">
                <td class="px-4 py-3 font-medium text-slate-900">{{ p.name }}</td>
                <td class="px-4 py-3 text-slate-700">{{ p.email }}</td>
                <td class="px-4 py-3 text-slate-700">{{ p.patientNumber }}</td>
                <td class="px-4 py-3 text-slate-700">{{ formatDate(p.dateOfBirth) }}</td>
              </tr>
            </tbody>
          </table>

          <!-- GP table -->
          <table v-else-if="tab==='gps'" class="w-full text-left text-sm">
            <thead class="border-b border-slate-200 bg-slate-50 text-slate-700">
              <tr>
                <th class="px-4 py-3">Naam</th>
                <th class="px-4 py-3">Email</th>
                <th class="px-4 py-3">Huisarts Id</th>
                <th class="px-4 py-3">Geboortedatum</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="!loading && filtered.length===0">
                <td class="px-4 py-4 text-slate-600" colspan="4">Geen resultaten.</td>
              </tr>
              <tr v-for="g in filtered" :key="g.id" class="border-b border-slate-100 hover:bg-slate-50">
                <td class="px-4 py-3 font-medium text-slate-900">{{ g.name }}</td>
                <td class="px-4 py-3 text-slate-700">{{ g.email }}</td>
                <td class="px-4 py-3 text-slate-700">{{ g.id }}</td>
                <td class="px-4 py-3 text-slate-700">{{ formatDate(g.dateOfBirth) }}</td>
              </tr>
            </tbody>
          </table>

          <!-- Staff table -->
          <table v-else class="w-full text-left text-sm">
            <thead class="border-b border-slate-200 bg-slate-50 text-slate-700">
              <tr>
                <th class="px-4 py-3">Naam</th>
                <th class="px-4 py-3">Email</th>
                <th class="px-4 py-3">EmployeeNumber</th>
                <th class="px-4 py-3">Specialisatie</th>
                <th class="px-4 py-3">Geboortedatum</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="!loading && filtered.length===0">
                <td class="px-4 py-4 text-slate-600" colspan="5">Geen resultaten.</td>
              </tr>
              <tr v-for="s in filtered" :key="s.employeeNumber" class="border-b border-slate-100 hover:bg-slate-50">
                <td class="px-4 py-3 font-medium text-slate-900">{{ s.name }}</td>
                <td class="px-4 py-3 text-slate-700">{{ s.email }}</td>
                <td class="px-4 py-3 text-slate-700">{{ s.employeeNumber }}</td>
                <td class="px-4 py-3 text-slate-700">{{ s.specialization }}</td>
                <td class="px-4 py-3 text-slate-700">{{ formatDate(s.dateOfBirth) }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="flex items-center justify-between px-4 py-3 text-xs text-slate-600">
          <span v-if="loading">Laden…</span>
          <span v-else>{{ filtered.length }} resultaten</span>
          <span>Tip: zoek op “@”, naam, nummer of specialisatie</span>
        </div>
      </div>
    </div>
  </div>
</template>
