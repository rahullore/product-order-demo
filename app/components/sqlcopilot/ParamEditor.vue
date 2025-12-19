
<script lang="ts" setup>

    import  type {ParamRow} from '~/composables/useSqlCopilot';

    const props = defineProps<{
        modelValue: ParamRow[];
    }>();

    const emit = defineEmits<{
        (e: 'update:modelValue', v: ParamRow[]): void;
    }>();

    const rows = computed({
        get: () => props.modelValue,
        set: (v: ParamRow[]) => emit('update:modelValue', v),
    });

    const tableRows = computed(() => {
        return rows.value.map((r, index) => ({
            name: r.name,
            value: r.value,
        }));
    });

    function add(){
        rows.value = [...rows.value, {id: crypto.randomUUID(), name: '', value: ''}];
    }

    function remove(index: number){
        const copy = [...rows.value];
        copy.splice(index, 1);
        rows.value = copy;
    }
</script>
<template>
  <div>
    <div class="flex items-center justify-between">
      <h3 class="text-sm font-medium text-gray-200">Parameters</h3>
      <button class="text-sm text-gray-300 hover:text-white underline underline-offset-4" @click="add">
        + Add
      </button>
    </div>

    <div class="mt-2">
      <UiDataTable :headers="['Name', 'Value']" :rows="tableRows" empty-text="No parameters">
        <template #actions="{ index }">
          <UiIconButton title="Remove" @click="remove(index)">✕</UiIconButton>
        </template>
      </UiDataTable>
    </div>

    <p class="mt-2 text-xs text-gray-500">
      Tip: values auto-parse (number/boolean/null/ISO date); otherwise string.
    </p>
  </div>
</template>