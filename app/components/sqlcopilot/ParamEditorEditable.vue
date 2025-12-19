<template>
  <div>
    <div class="flex items-center justify-between">
      <h3 class="text-sm font-medium text-gray-200">Parameters</h3>
      <button class="text-sm text-gray-300 hover:text-white underline underline-offset-4" @click="add">
        + Add
      </button>
    </div>

    <div class="mt-2 overflow-x-auto rounded-xl border border-gray-800">
      <table class="w-full text-sm">
        <thead class="bg-gray-900">
          <tr class="text-left text-gray-300">
            <th class="px-3 py-2 w-1/3">Name</th>
            <th class="px-3 py-2 w-2/3">Value</th>
            <th class="px-3 py-2 w-[64px]"></th>
          </tr>
        </thead>

        <tbody class="bg-gray-950">
          <tr v-if="rows.length === 0" class="border-t border-gray-800">
            <td class="px-3 py-3 text-gray-500" colspan="3">No parameters</td>
          </tr>

          <SqlcopilotParamRowItem
            v-for="(r, idx) in rows"
            :key="r.id"
            :row="r"
            @update="updateRow(idx, $event)"
            @remove="remove(idx)"
          />
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ParamRow } from "~/composables/useSqlCopilot";

const props = defineProps<{ modelValue: ParamRow[] }>();
const emit = defineEmits<{ (e: "update:modelValue", v: ParamRow[]): void }>();

const rows = computed({
  get: () => props.modelValue,
  set: (v) => emit("update:modelValue", v),
});

function add() {
  rows.value = [...rows.value, { id: crypto.randomUUID(), name: "", value: "" }];
}
function remove(idx: number) {
  const copy = [...rows.value];
  copy.splice(idx, 1);
  rows.value = copy;
}
function updateRow(idx: number, row: ParamRow) {
  const copy = [...rows.value];
  copy[idx] = row;
  rows.value = copy;
}
</script>
