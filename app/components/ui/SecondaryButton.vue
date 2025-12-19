<script lang="ts" setup>

    defineProps<{
        disabled?: boolean;
        type?: 'button' | 'submit';
    }>();

    defineEmits<{
        (e: 'click'): void;
    }>();
</script>
<template>
    <button
        :disabled="disabled"
        :type="type || 'button'"
        class="rounded-xl border border-gray-700 px-4 py-2 text-sm disabled:opacity-60"
        @click="$emit('click')">
        <slot/>
    </button>
</template>