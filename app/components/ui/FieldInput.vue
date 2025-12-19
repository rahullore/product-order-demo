<script lang="ts" setup>

    const props = defineProps<{
        label: string;
        modelValue:string;
        placeHolder?: string;
    }>();
    
    const emit = defineEmits<{
        (e: 'update:modelValue', v: string): void;
    }>();

</script>
<template>

    <label class="text-sm text-gray-300">
        {{ label }}
        <input
            type="text"
            :placeholder="placeHolder || ''"
            class="mt-1 w-full rounded-xl bg-gray-950 border border-gray-800 p-2 text-sm outline-none focus:ring-2 focus:ring-gray-700"
            :value="modelValue"
            @input="(e) => emit('update:modelValue', (e.target as HTMLInputElement).value)"
        />
    </label>


</template>