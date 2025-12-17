<script setup lang="ts">
const props = defineProps<{
    modelValue: string
    loading?: boolean
}>();

const emits = defineEmits<{
    (e: 'update:modelValue', value: string): void
    (e: 'search', value: string): void
}>();


function onKeydown(e: KeyboardEvent) {
   if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
        emits('search', props.modelValue);
   }
}
</script>

<template>
    <div class="rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-900 p-4">
        <label class="block text-sm font-medium text-gray-700 dark:text-slate-200 mb-2">
            Ask about your orders
        </label>

        <textarea
            :value="modelValue"
            @input="emits('update:modelValue', ($event.target as HTMLTextAreaElement).value)"
            @keydown="onKeydown"
            rows="3"
            class="w-full rounded-md border border-gray-300 dark:border-slate-600 bg-white dark:bg-slate-950 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="e.g., least expensive product • most ordered product • orders about infusion pump"
        ></textarea>

        <div class="mt-3 flex items-center justify-between gap-3">
            <p class="text-xs text-gray-500 dark:text-slate-400">
                Tip: Use Ctrl + Enter (or Cmd + Enter on Mac) to submit your query.
            </p>

            <button
                class="rounded-md bg-blue-600 text-white px-4 py-2 text-sm font-medium hover:bg-blue-700 disabled:opacity-60"
                :disabled="loading"
                @click="emits('search', props.modelValue)"
            >
                {{ loading ? 'Searching…' : 'Search' }}
      </button>
        </div>
    </div>
</template>