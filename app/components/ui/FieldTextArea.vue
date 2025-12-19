<script lang="ts" setup>
    const props = defineProps<{
        modelValue: string;
        placeholder?: string;
        rows?: number;
        label: string;
    }>();

    const emit = defineEmits<{
        (e: 'update:modelValue', v: string): void;
    }>();

    const local = computed({
        get: () => props.modelValue,
        set: (v: string) => emit('update:modelValue', v),
    });

    function onInput(e: Event) {
        emit("update:modelValue", (e.target as HTMLTextAreaElement).value);
    }
</script>

<template>
    <label class="text-sm text-gray-300">
        {{ label }}
        <textarea
            :placeholder="placeholder || ''"
            :rows="rows || 6"
            class="mt-1 w-full rounded-xl bg-gray-950 border border-gray-800 p-3 text-sm outline-none focus:ring-2 focus:ring-gray-700"
            v-model="local"
            
        ></textarea>
    </label>
</template>