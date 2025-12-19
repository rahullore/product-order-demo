<template>
  <UiSectionCard title="3) Result">
    <div v-if="result" class="flex flex-wrap items-center gap-2">
      <UiStatChip>Success: <b>{{ result.success }}</b></UiStatChip>
      <UiStatChip>Kind: <b>{{ result.kind }}</b></UiStatChip>
      <UiStatChip>AffectedRows: <b>{{ result.affectedRows }}</b></UiStatChip>
    </div>

    <div v-if="result?.rows?.length" class="mt-4">
      <UiDataTable :headers="headers" :rows="result.rows" />
    </div>

    <div v-else class="mt-4 text-sm text-gray-400">
      <span v-if="result">No rows returned.</span>
      <span v-else>Nothing executed yet.</span>
    </div>
  </UiSectionCard>
</template>

<script setup lang="ts">
type SqlExecuteResponse = {
  success: boolean;
  kind: string;
  affectedRows: number;
  rows?: Array<Record<string, any>> | null;
};

const props = defineProps<{ result: SqlExecuteResponse | null }>();

const headers = computed(() => {
  const rows = props.result?.rows ?? [];
  if (!rows.length) return [];
  const set = new Set<string>();
  for (const r of rows) Object.keys(r).forEach((k) => set.add(k));
  return Array.from(set);
});
</script>
