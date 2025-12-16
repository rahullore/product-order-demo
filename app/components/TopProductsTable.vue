<script setup lang="ts">

type TopProduct = {
    name:string;
    totalQty:number;
    totalSpend:number;
};

defineProps<{
    items: TopProduct[];
}>();

const money = (n: number) => {
    console.log(n);
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(n);
}

</script>

<template>
    <div class="rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-900 p-4">
        <h3 class="text-sm font-semibold text-gray-900 dark:text-slate-100 mb-3">Top Products</h3>
        <div v-if="!items.length" class="text-sm text-gray-600 dark:text-slate-300">
            No orders yet.
        </div>

        <table v-else class="w-full text-sm">
            <thead class="text-left text-gray-600 dark:text-slate-300">
                <tr>
                    <th class="pb-2">Product</th>
                    <th class="pb-2">Total Qty</th>
                    <th class="pb-2">Total Spend</th>
                </tr>
                <tr v-for="item in items" :key="item.name" class="border-t border-gray-100 dark:border-slate-800">
                    <td class="pt-2">{{ item.name }}</td>
                    <td class="pt-2">{{ item.totalQty }}</td>
                    <td class="pt-2">{{ money(item.totalSpend) }}</td>
                </tr>
            </thead>
        </table>
    </div>
</template>