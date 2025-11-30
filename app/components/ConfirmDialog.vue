<script setup lang="ts">
const props = defineProps<{
    title: string
    message: string
    confirmLabel?: string
    cancelLabel?: string    
}>();

const emit = defineEmits<{
    (e: "confirm"): void
    (e: "cancel"): void
}>();

</script>

<template>
    <div class="fixed inset-0 bg-black/40 flex items-center justify-center z-50 ">
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-lg p-6 w-80">
            <h2 class="text-lg font-semibold mb-2">
                {{ title }}
            </h2>
            <p class="text-sm text-gray-700 dark:text-slate-200 mb-4">
                {{ message }}
            </p>

            <div class="flex justify-end gap-2">
                <button
                    type="button"
                    class="px-3 py-1 rounded-md text-sm border border-gray-300 dark:border-slate-600 hover:bg-gray-100 dark:hover:bg-slate-700"
                    @click="$emit('cancel')">
                    {{ cancelLabel || 'Cancel' }}
                </button>
                <button
                    type="button"
                     class="px-3 py-1 rounded-md text-sm bg-red-600 text-white hover:bg-red-700"
                    @click="$emit('confirm')">
                    {{ confirmLabel || 'Delete' }}
                </button>
            </div>
        </div>
    </div>
</template>

