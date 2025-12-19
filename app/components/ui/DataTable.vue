<script lang="ts" setup>
   const props = defineProps<{
    headers: string[];
    rows: Array<Record<string, any>>;
    emptyText?: string; 
   }>();

   function format(v:any){
    if (v === null || v === undefined) return '';
    if (typeof v === 'object') return JSON.stringify(v);
    return String(v);
   }
</script>
<template>
    <div class="overflow-x-auto rounded-xl border border-gray-800">
        <table class="w-full text-sm">
            <thead class="bg-gray-900">
                <tr>
                    <th v-for="h in headers" :key="h" class="px-3 py-2">
                        {{ h }}
                    </th>
                    <th v-if="$slots.actions" class="px-3 py-2 w-[64px]"></th>
                </tr>
            </thead>

            <tbody class="bg-gray-950">
                <tr v-if="rows.length === 0" class="border-t border-gray-800">
                    <td class="px-3 py-3 text-gray-500" :colspan="headers.length + ($slots.actions ? 1 : 0)">
                        {{ emptyText ?? "No data" }}
                    </td>
                </tr>

                <tr v-for="(r, i) in rows" :key="i" class="border-t border-gray-800">
                    <td v-for="h in headers" :key="h" class="px-3 py-2 text-gray-200">
                        {{ format(r[h]) }}
                    </td>

                <td v-if="$slots.actions" class="px-3 py-2 text-right">
                    <slot name="actions" :row="r" :index="i" />
                </td>
                </tr>
            </tbody>

        </table>
    </div>

</template>