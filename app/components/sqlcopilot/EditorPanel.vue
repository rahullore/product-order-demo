<template>
  <UiSectionCard title="2) Review & Execute">
    <label class="block mb-2 text-sm font-medium text-gray-300">SQL Editor</label>
    <div class="grid gap-3">
      <UiFieldInput label="Table Name" v-model="table" placeholder="dbo.Products" />
      <UiFieldTextArea label="SQL" v-model="sqlText" :rows="8" placeholder="SQL appears here..." />
    </div>

    <div class="mt-4">
      <SqlcopilotParamEditorEditable v-model="paramsRows" />
    </div>

    <div class="mt-4 flex items-center gap-3">
      <button
        class="rounded-xl bg-emerald-400 text-gray-950 px-4 py-2 text-sm font-medium disabled:opacity-60"
        :disabled="isLoading || !sqlText.trim() || !table.trim()"
        @click="$emit('execute')"
      >
        {{ isLoading ? "Executing..." : "Execute" }}
      </button>

      <span v-if="error" class="text-sm text-red-400">{{ error }}</span>
    </div>
  </UiSectionCard>
</template>

<script setup lang="ts">
import type { ParamRow } from "~/composables/useSqlCopilot";

const props = defineProps<{
  tableName: string;
  sql: string;
  parameters: ParamRow[];
  isLoading: boolean;
  error: string | null;
}>();

const emit = defineEmits<{
  (e: "update:tableName", v: string): void;
  (e: "update:sql", v: string): void;
  (e: "update:parameters", v: ParamRow[]): void;
  (e: "execute"): void;
}>();

const table = computed({
  get: () => props.tableName,
  set: (v) => emit("update:tableName", v),
});
const sqlText = computed({
  get: () => props.sql,
  set: (v) => emit("update:sql", v),
});
const paramsRows = computed({
  get: () => props.parameters,
  set: (v) => emit("update:parameters", v),
});
</script>
