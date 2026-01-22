<template>
  <section class="space-y-3">
    <header class="space-y-1">
      <h3 class="text-lg font-semibold">Actieve doorverwijzingen</h3>
      <p class="text-sm text-gray-600">
        Selecteer een doorverwijzing. Zonder doorverwijzing kun je geen afspraak maken.
      </p>
    </header>

    <div class="space-y-2">
      <label class="block text-sm font-medium">Zoek</label>
      <input
        class="w-full rounded-xl border px-3 py-2"
        type="text"
        v-model="search"
        :disabled="loading"
        placeholder="Zoek op behandeling of care code"
      />
    </div>

    <div class="space-y-2">
      <label class="block text-sm font-medium">Doorverwijzing</label>

      <div class="flex gap-2">
        <select
          class="w-full rounded-xl border px-3 py-2"
          :disabled="loading || filtered.length === 0"
          v-model="selectedId"
        >
          <option value="" disabled>
            {{ loading ? "Laden..." : "Kies een doorverwijzing" }}
          </option>

          <option v-for="r in filtered" :key="r.id" :value="String(r.id)">
            {{ label(r) }}
          </option>
        </select>

        <button
          class="rounded-xl border px-3 py-2"
          type="button"
          @click="$emit('refresh')"
          :disabled="loading"
        >
          Vernieuwen
        </button>
      </div>

      <p v-if="error" class="text-sm text-red-600">{{ error }}</p>

      <p v-if="!loading && !error && filtered.length === 0" class="text-sm text-gray-600">
        Geen actieve doorverwijzingen. Neem contact op met je huisarts.
      </p>

      <div v-if="selectedReferral" class="rounded-xl border p-3 text-sm">
        <div class="font-medium">Geselecteerd</div>
        <div class="text-gray-600">
          Behandeling: <span class="font-medium">{{ selectedReferral.treatmentDescription }}</span>
        </div>
        <div class="text-gray-500 text-xs">
          Care code: <span class="font-mono">{{ selectedReferral.careCode }}</span>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed, ref, watch } from "vue";

const props = defineProps({
  referrals: { type: Array, default: () => [] },
  loading: { type: Boolean, default: false },
  error: { type: String, default: "" },
  selectedReferral: { type: Object, default: null }
});

const emit = defineEmits(["refresh", "update:selectedReferral"]);

const search = ref("");
const selectedId = ref("");

function label(r) {
  const desc = r.treatmentDescription || "";
  return `${desc} (${r.careCode})`;
}

const filtered = computed(() => {
  const q = search.value.trim().toLowerCase();
  if (!q) return props.referrals;

  return props.referrals.filter(r => {
    const hay = `${r.treatmentDescription ?? ""} ${r.careCode ?? ""}`.toLowerCase();
    return hay.includes(q);
  });
});

// sync incoming selectedReferral -> selectedId
watch(
  () => props.selectedReferral,
  (val) => {
    selectedId.value = val?.id ? String(val.id) : "";
  },
  { immediate: true }
);

// when selection changes -> emit selected referral object
watch(selectedId, (id) => {
  if (!id) {
    emit("update:selectedReferral", null);
    return;
  }
  const found = props.referrals.find(r => String(r.id) === String(id)) ?? null;
  emit("update:selectedReferral", found);
});
</script>
