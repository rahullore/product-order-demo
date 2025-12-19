<script lang="ts" setup>
    defineProps<{
        title?: string;
    }>();

    defineEmits<{
        (e: 'click'): void;
    }>();
</script>

<template>
    <button
        :title="title"
        class="px-2 py-1 rounded-lg hover:bg-gray-900 text-sm"
        @click="$emit('click')"
    >
        <slot />
    </button>
</template>