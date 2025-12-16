<script setup lang="ts">

defineProps<{
    label: string;
    value: string;
    hint?: string;
}>();


</script>

<template>
    <div class="rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-900 p-4">
        <div class="text-xs text-gray-500 dark:text-slate-400">{{  label   }}</div>
        <div class="mt-1 text-xl font-semibold text-gray-900 dark:text-slate-100">{{ value }}</div>
        <div v-if="hint" class="mt-1 text-xs text-gray-600 dark:text-slate-300">{{ hint }}</div>
    </div>
</template>