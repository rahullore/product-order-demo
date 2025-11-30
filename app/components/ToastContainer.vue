<script setup lang="ts">
const {toasts,removeToast}=useToasts();
</script>

<template>
    <div class="fixed bottom-4 right-4 space-y-2 z-50">
        <div
            v-for="toast in toasts"
            :key="toast.id"
            class="min-w-[220px] max-w-xs rounded-md shadow-lg px-4 py-3 text-sm text-white cursor-pointer"
            :class="{
                'bg-green-600': toast.type === 'success',
                'bg-red-600': toast.type === 'error',
                'bg-blue-800': toast.type === 'info',
            }"
            @click="removeToast(toast.id)">
            {{ toast.message }}
        </div>
    </div>
</template>