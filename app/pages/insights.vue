<script setup lang="ts">
import { tr } from '@nuxt/ui/runtime/locale/index.js';
import type InsightCardVue from '~/components/InsightCard.vue';
import type TopProductsTableVue from '~/components/TopProductsTable.vue';
import { useInsights } from '~/composables/useInsights';

const {fetchInsights} =  useInsights();

useHead({
    title: 'Insights - Customer Orders App',
    meta: [
        { name: 'description', content: 'Order insights summary.' }
    ],
});

const loading = ref(true);
const errorMsg = ref<string | null>(null);
const insights = ref<Awaited<ReturnType<typeof fetchInsights>> | null>(null);

const money = (n: number) => {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(n);
}

const fmtTime = (iso:string) => new Date(iso).toLocaleDateString();

async function load(){
    loading.value = true;
    errorMsg.value = null;
    try {
        insights.value = await fetchInsights();
    } catch (error: any) {
        errorMsg.value = error?.data?.message || error.message ||('An error occurred while fetching insights.');
    } finally {
        loading.value = false;
    }
}

onMounted(load);

</script>

<template>
    <section class="max-w-4xml mx-auto">
        <div class="flex item-start justify-between gap-4 mb-6">
            
            <div>
                <h2 class="text-xl font-semibold">Order Insights </h2>
                <p class="text-sm text-gray-600 dark:text-slate-300"> Quick summary of your orders (deterministic, no AI).</p>
                <button
                    @click="load"
                    :disabled="loading"
                    class="rounded-md border border-gray-300 dark:border-slate-700 px-4 py-2 text-sm hover:bg-gray-50 dark:hover:bg-slate-800">
                    {{ loading ? 'Loading...' : 'Refresh' }}
                </button>
            </div>
        
            <div v-if="loading" class="text-sm text-gray-600 dark:text-slate-300">Loading insights... </div>
            <div v-else-if="errorMsg" class="text-sm text-red-600 dark:text-red-400">{{ errorMsg }}</div>

            <div v-else-if="insights" class="space-y-4">
                <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                    <InsightCard label="Total Orders" :value="String(insights.totalOrders)" />
                    <InsightCard label="Total Items" :value="String(insights.totalItems)" />
                    <InsightCard label="Total Spend" :value="money(insights.totalSpend)" />
                    <InsightCard label="Last Updated" :value="fmtTime(insights.lastUpdateUTC)" hint="Server Time" />
                </div>
                <TopProductsTable :items="insights.topProducts" />
            </div>
        </div>
    </section>
</template>