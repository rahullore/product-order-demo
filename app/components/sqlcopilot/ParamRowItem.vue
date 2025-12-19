<template>
  <tr class="border-t border-gray-800">
    <td class="px-3 py-2">
      <input
        v-model="local.name"
        class="w-full rounded-lg bg-gray-900 border border-gray-800 p-2 outline-none focus:ring-2 focus:ring-gray-700"
        placeholder="name (or @name)"
      />
    </td>
    <td class="px-3 py-2">
      <input
        v-model="local.value"
        class="w-full rounded-lg bg-gray-900 border border-gray-800 p-2 outline-none focus:ring-2 focus:ring-gray-700"
        placeholder="Demo Pump / 123.45 / true / 2025-01-01"
      />
    </td>
    <td class="px-3 py-2 text-right">
      <UiIconButton title="Remove" @click="$emit('remove')">✕</UiIconButton>
    </td>
  </tr>
</template>

<script setup lang="ts">
import type { ParamRow } from "~/composables/useSqlCopilot";

const props = defineProps<{ row: ParamRow }>();
const emit = defineEmits<{
  (e: "update", row: ParamRow): void;
  (e: "remove"): void;
}>();

const local = computed({
  get: () => props.row,
  set: (v) => emit("update", v),
});
</script>
