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
        class="rounded-xl bg-white text-gray-900 px-4 py-2 text-sm font-medium disabled:opacity-60"
        @click="$emit('click')">
        <slot/>
    </button>
</template>