

<template>
  <UiSectionCard title="1) Instruction">
    <UiFieldTextArea
      label=""
      v-model="instruction"
      :rows="6"
      :placeholder="placeholder"
    />

    <div class="mt-4 flex items-center gap-3">
      <UiPrimaryButton :disabled="isLoading || !instruction.trim()" @click="$emit('generate')">
        {{ isLoading ? "Generating..." : "Generate" }}
      </UiPrimaryButton>

      <UiSecondaryButton :disabled="isLoading" @click="$emit('reset')">Reset</UiSecondaryButton>

      <span v-if="error" class="text-sm text-red-400">{{ error }}</span>
    </div>

    <div v-if="summary" class="mt-5 rounded-xl border border-gray-800 bg-gray-950 p-4">
      <div class="flex flex-wrap items-center gap-2">
        <UiStatChip>Kind: <b>{{ summary.kind }}</b></UiStatChip>
        <UiStatChip>Table: <b>{{ summary.tableName }}</b></UiStatChip>
      </div>
      <p v-if="summary.notes" class="mt-3 text-sm text-gray-300">
        <span class="text-gray-400">Notes:</span> {{ summary.notes }}
      </p>
    </div>
  </UiSectionCard>
</template>

<script setup lang="ts">
   
const props = defineProps<{
  modelValue: string;
  isLoading: boolean;
  error: string | null;
  summary: { kind: string; tableName: string; notes: string } | null;
  placeholder?: string;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", v: string): void;
  (e: "generate"): void;
  (e: "reset"): void;
}>();

const instruction = computed({
  get: () => props.modelValue,
  set: (v) => emit("update:modelValue", v),
});
</script>
