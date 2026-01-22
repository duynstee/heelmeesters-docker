<template>
  <div class="space-y-2">
    <p v-if="loading" class="text-sm text-gray-600">Afspraken laden...</p>
    <p v-if="error" class="text-sm text-red-600">{{ error }}</p>

    <p v-if="!loading && !error && appointments.length === 0" class="text-sm text-gray-600">
      Geen afspraken gevonden.
    </p>

    <ul v-if="appointments.length > 0" class="space-y-2">
      <li v-for="a in appointments" :key="a.id" class="rounded-xl border p-3">
        <div class="flex items-center justify-between gap-3">
          <div>
            <div class="font-medium">Afspraak #{{ a.id }}</div>
            <div class="text-sm text-gray-600">
              Care code: <span class="font-mono">{{ a.careCode }}</span> • Room:
              <span class="font-mono">{{ a.roomCode }}</span>
            </div>
            <div class="text-xs text-gray-500">
              {{ fmt(a.startTime) }} → {{ fmt(a.endTime) }}
            </div>
          </div>
        </div>
      </li>
    </ul>
  </div>
</template>

<script setup>
const props = defineProps({
  appointments: { type: Array, default: () => [] },
  loading: { type: Boolean, default: false },
  error: { type: String, default: "" }
});

function fmt(iso) {
  try {
    return new Date(iso).toLocaleString();
  } catch {
    return iso;
  }
}
</script>
