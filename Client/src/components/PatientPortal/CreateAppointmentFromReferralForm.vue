<template>
  <section class="space-y-4">
    <header class="space-y-1">
      <h3 class="text-lg font-semibold">Afspraak plannen</h3>
      <p class="text-sm text-gray-600">
        Je kunt alleen plannen met een geselecteerde doorverwijzing en binnen 09:00–17:00.
      </p>
    </header>

    <p v-if="!selectedReferral" class="text-sm text-gray-600">
      Selecteer eerst een doorverwijzing hierboven.
    </p>

    <div v-else class="space-y-4">
      <div class="rounded-xl border p-3 text-sm">
        <div class="font-medium">Doorverwijzing</div>
        <div class="text-gray-600">
          Behandeling: <span class="font-medium">{{ selectedReferral.treatmentDescription }}</span>
        </div>
        <div class="text-gray-500 text-xs">
          Care code: <span class="font-mono">{{ selectedReferral.careCode }}</span>
        </div>
      </div>

      <!-- Doctor + Room dropdowns -->
      <div class="grid gap-3 md:grid-cols-2">
        <div class="space-y-1">
          <label class="block text-sm font-medium">Dokter</label>
          <select
            class="w-full rounded-xl border px-3 py-2"
            v-model="selectedDoctor"
            :disabled="loadingDoctors || saving"
          >
            <option value="" disabled>
              {{ loadingDoctors ? "Dokters laden..." : "Kies een dokter" }}
            </option>
            <option v-for="d in doctors" :key="d.employeeNumber" :value="String(d.employeeNumber)">
              {{ d.name }}{{ d.specialization ? ` — ${d.specialization}` : "" }}
            </option>
          </select>
          <p v-if="doctorsError" class="text-sm text-red-600">{{ doctorsError }}</p>
        </div>

        <div class="space-y-1">
          <label class="block text-sm font-medium">Kamer</label>
          <select
            class="w-full rounded-xl border px-3 py-2"
            v-model="selectedRoom"
            :disabled="loadingRooms || saving"
          >
            <option value="" disabled>
              {{ loadingRooms ? "Kamers laden..." : "Kies een kamer" }}
            </option>
            <option v-for="r in rooms" :key="r.roomCode" :value="r.roomCode">
              {{ r.roomCode }}
            </option>
          </select>
          <p v-if="roomsError" class="text-sm text-red-600">{{ roomsError }}</p>
        </div>
      </div>

      <!-- Week selector -->
      <div class="grid gap-3 md:grid-cols-2">
        <div class="space-y-1">
          <label class="block text-sm font-medium">Week (maandag)</label>
          <input
            class="w-full rounded-xl border px-3 py-2"
            type="date"
            v-model="weekStartDate"
            :disabled="saving"
          />
          <p class="text-xs text-gray-500">
            Tip: kies de maandag van de week.
          </p>
        </div>

        <div class="space-y-1">
          <label class="block text-sm font-medium">Duur</label>
          <select class="w-full rounded-xl border px-3 py-2" v-model="durationMinutes" :disabled="saving">
            <option :value="15">15 min</option>
            <option :value="30">30 min</option>
            <option :value="45">45 min</option>
            <option :value="60">60 min</option>
          </select>
        </div>
      </div>

      <!-- Availability overview -->
      <div class="rounded-2xl border p-4 space-y-3">
        <div class="flex items-center justify-between gap-2">
          <div>
            <div class="font-medium">Beschikbaarheid</div>
            <div class="text-sm text-gray-600">
              Kies een tijdslot. Bezet = uitgegrijsd.
            </div>
          </div>
          <button
            class="rounded-xl border px-3 py-2 text-sm"
            type="button"
            @click="loadSchedule"
            :disabled="saving || !selectedDoctor || loadingSchedule"
          >
            {{ loadingSchedule ? "Laden..." : "Verversen" }}
          </button>
        </div>

        <p v-if="scheduleError" class="text-sm text-red-600">{{ scheduleError }}</p>

        <div v-if="!selectedDoctor" class="text-sm text-gray-600">
          Selecteer eerst een dokter om bezette tijden te zien.
        </div>

        <div v-else class="grid gap-3 md:grid-cols-2">
          <div class="space-y-2">
            <label class="block text-sm font-medium">Dag</label>
            <select class="w-full rounded-xl border px-3 py-2" v-model="selectedDay" :disabled="saving">
              <option v-for="d in weekDays" :key="d.iso" :value="d.iso">
                {{ d.label }}
              </option>
            </select>
          </div>

          <div class="space-y-2">
            <label class="block text-sm font-medium">Tijdslot</label>
            <select
              class="w-full rounded-xl border px-3 py-2"
              v-model="selectedSlot"
              :disabled="saving || slotsForSelectedDay.length === 0"
            >
              <option value="" disabled>Kies een tijd</option>
              <option
                v-for="s in slotsForSelectedDay"
                :key="s.value"
                :value="s.value"
                :disabled="s.disabled"
              >
                {{ s.label }}{{ s.disabled ? " (bezet)" : "" }}
              </option>
            </select>

            <p v-if="slotsForSelectedDay.length === 0" class="text-sm text-gray-600">
              Geen tijdsloten beschikbaar.
            </p>
          </div>
        </div>
      </div>

      <!-- Submit -->
      <div class="flex items-center gap-2">
        <button
          class="rounded-xl border px-4 py-2"
          type="button"
          @click="submit"
          :disabled="saving || !canSubmit"
        >
          {{ saving ? "Opslaan..." : "Afspraak aanmaken" }}
        </button>

        <span v-if="success" class="text-sm text-green-700">{{ success }}</span>
      </div>

      <p v-if="error" class="text-sm text-red-600">{{ error }}</p>
    </div>
  </section>
</template>

<script setup>
import { computed, onMounted, ref, watch } from "vue";
import {
  createAppointmentFromReferral,
  getDoctorWeekSchedule,
  getPatientDoctors,
  getPatientRooms
} from "../../services/patientService";

const props = defineProps({
  selectedReferral: { type: Object, default: null }
});
const emit = defineEmits(["created"]);

const doctors = ref([]);
const rooms = ref([]);
const loadingDoctors = ref(false);
const loadingRooms = ref(false);
const doctorsError = ref("");
const roomsError = ref("");

const selectedDoctor = ref("");
const selectedRoom = ref("");

const weekStartDate = ref(""); // YYYY-MM-DD
const durationMinutes = ref(30);

const schedule = ref([]); // [{startTime,endTime}]
const loadingSchedule = ref(false);
const scheduleError = ref("");

const selectedDay = ref("");
const selectedSlot = ref(""); // "HH:mm"

const saving = ref(false);
const error = ref("");
const success = ref("");

// helpers
function todayIso() {
  const d = new Date();
  const yyyy = d.getFullYear();
  const mm = String(d.getMonth() + 1).padStart(2, "0");
  const dd = String(d.getDate()).padStart(2, "0");
  return `${yyyy}-${mm}-${dd}`;
}

// zet default weekStart op vandaag (maar jij kiest maandag)
onMounted(async () => {
  weekStartDate.value = todayIso();
  await loadDoctors();
  await loadRooms();
});

async function loadDoctors() {
  doctorsError.value = "";
  loadingDoctors.value = true;
  try {
    doctors.value = await getPatientDoctors();
  } catch (e) {
    doctorsError.value = e?.response?.data ?? e?.message ?? "Kon dokters niet ophalen.";
  } finally {
    loadingDoctors.value = false;
  }
}

async function loadRooms() {
  roomsError.value = "";
  loadingRooms.value = true;
  try {
    rooms.value = await getPatientRooms();
  } catch (e) {
    roomsError.value = e?.response?.data ?? e?.message ?? "Kon kamers niet ophalen.";
  } finally {
    loadingRooms.value = false;
  }
}

const weekDays = computed(() => {
  const start = weekStartDate.value ? new Date(weekStartDate.value) : new Date();
  const base = new Date(start);
  base.setHours(0, 0, 0, 0);

  // 7 dagen labels
  const labels = ["Zo", "Ma", "Di", "Wo", "Do", "Vr", "Za"];
  return Array.from({ length: 7 }).map((_, i) => {
    const d = new Date(base);
    d.setDate(base.getDate() + i);
    const iso = d.toISOString().slice(0, 10);
    return { iso, label: `${labels[d.getDay()]} ${iso}` };
  });
});

watch(weekDays, (days) => {
  if (!selectedDay.value && days.length) selectedDay.value = days[0].iso;
}, { immediate: true });

watch([selectedDoctor, weekStartDate], async () => {
  // als dokter of week verandert: schedule reset + slot reset
  schedule.value = [];
  scheduleError.value = "";
  selectedSlot.value = "";
  if (selectedDoctor.value) {
    await loadSchedule();
  }
});

async function loadSchedule() {
  scheduleError.value = "";
  loadingSchedule.value = true;
  schedule.value = [];

  try {
    const list = await getDoctorWeekSchedule({
      employeeNumber: Number(selectedDoctor.value),
      weekStartIso: weekStartDate.value
    });
    schedule.value = Array.isArray(list) ? list : [];
  } catch (e) {
    scheduleError.value = e?.response?.data ?? e?.message ?? "Kon rooster niet ophalen.";
  } finally {
    loadingSchedule.value = false;
  }
}

function parseIso(iso) {
  const d = new Date(iso);
  return isNaN(d.getTime()) ? null : d;
}

function overlaps(startA, endA, startB, endB) {
  // overlap als A start < B end en A end > B start
  return startA < endB && endA > startB;
}

// fixed slots per dag (09:00–17:00) met stappen van 30 minuten
function generateSlotsForDay(dayIso, durationMin) {
  const openMin = 9 * 60;
  const closeMin = 17 * 60;

  const step = 30; // ✅ jouw vaste tijden (14:00, 14:30, 16:30, etc.)
  const slots = [];

  for (let m = openMin; m + durationMin <= closeMin; m += step) {
    const hh = String(Math.floor(m / 60)).padStart(2, "0");
    const mm = String(m % 60).padStart(2, "0");
    const value = `${hh}:${mm}`;
    const label = value;

    // maak start/end Date voor overlap check
    const start = new Date(`${dayIso}T${value}:00`);
    const end = new Date(start);
    end.setMinutes(end.getMinutes() + durationMin);

    // check overlap met schedule items
    const isBusy = schedule.value.some(x => {
      const s = parseIso(x.startTime);
      const e = parseIso(x.endTime);
      if (!s || !e) return false;
      return overlaps(start, end, s, e);
    });

    slots.push({ value, label, disabled: isBusy });
  }

  return slots;
}

const slotsForSelectedDay = computed(() => {
  if (!selectedDay.value) return [];
  return generateSlotsForDay(selectedDay.value, Number(durationMinutes.value));
});

const canSubmit = computed(() => {
  return !!props.selectedReferral?.id &&
    !!selectedDoctor.value &&
    !!selectedRoom.value &&
    !!selectedDay.value &&
    !!selectedSlot.value;
});

function toIso(dayIso, hhmm) {
  return new Date(`${dayIso}T${hhmm}:00`).toISOString();
}

async function submit() {
  error.value = "";
  success.value = "";

  if (!canSubmit.value) {
    error.value = "Vul alles in (doorverwijzing, dokter, kamer, dag en tijd).";
    return;
  }

  // extra client check: gekozen slot moet niet disabled zijn
  const chosen = slotsForSelectedDay.value.find(s => s.value === selectedSlot.value);
  if (!chosen || chosen.disabled) {
    error.value = "Dit tijdslot is bezet. Kies een ander tijdslot.";
    return;
  }

  const startIso = toIso(selectedDay.value, selectedSlot.value);
  const start = new Date(startIso);
  const end = new Date(start);
  end.setMinutes(end.getMinutes() + Number(durationMinutes.value));
  const endIso = end.toISOString();

  saving.value = true;
  try {
    await createAppointmentFromReferral({
      referralId: props.selectedReferral.id,
      employeeNumber: Number(selectedDoctor.value),
      roomCode: selectedRoom.value,
      startTime: startIso,
      endTime: endIso
    });

    success.value = "Afspraak aangemaakt.";
    emit("created");

    // reset slot keuze (doctor/room laten staan)
    selectedSlot.value = "";
  } catch (e) {
    error.value = e?.response?.data ?? e?.message ?? "Aanmaken mislukt.";
  } finally {
    saving.value = false;
  }
}

watch(() => props.selectedReferral?.id, () => {
  error.value = "";
  success.value = "";
});
</script>
