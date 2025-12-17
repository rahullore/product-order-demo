<script setup lang="ts">
    const{post} = useApi();

    type VectorSearchResponseItem = {
        id: string;
        text: string;
        score: number;
        metadata: any;
    }

    

    type VectorBuildIndexResponse = {
        indexedOrders: number;
        indexedProducts: number;
        totalVectors: number;
    }

    useHead({
        title: 'Smart Search - Customer Orders App',
        meta: [
            { name: 'description', content: 'Search your customer orders using embeddings.' }
        ],
    })

    const query = ref('least expensive product');
    const topK = ref(5);
    const loading = ref(false);
    const results = ref<VectorSearchResponseItem[]>([]);
    const errorMsg = ref<string | null>(null);


    function parseTitle(item: VectorSearchResponseItem){
        return item.metadata.productName || item.id;

    }

    function parseSubtitle(item: VectorSearchResponseItem){
        if (item.metadata.unitPrice) return `Unit price: $${item.metadata.unitPrice}`
        if (item.metadata.quantity) return `Quantity: ${item.metadata.quantity}`
        if(item.metadata.totalQuantity) return `Total Quantity: ${item.metadata.totalQuantity}`
        if(item.metadata.totalSpend) return `Total Spend: $${item.metadata.totalSpend}`
        if(item.metadata.orderCount) return `Order Count: ${item.metadata.orderCount}`
        // fallback: show the first part of text
        return item.text.length > 80 ? item.text.slice(0, 80) + '…' : item.text
    }

    async function BuildIndex(){
        try {
            var res = await post<VectorBuildIndexResponse>('/api/vector/build-index', {});
            console.log('Index build result:', res);
        } catch (err:any) {
            console.error('Build index error:', err);
            errorMsg.value = err?.message || 'An error occurred during index building.';
        } 
    }

    async function runSearch(){
        const q = query.value.trim();
        if (!q) return;

        loading.value = true;
        errorMsg.value = null;
        results.value = [];

        try {
            await BuildIndex();
            const res = await post<VectorSearchResponseItem[]>('/api/vector/search', {
                query: q,
                topK: topK.value,
            });
            console.log('Search result:', res);

            results.value = res;
        } catch (err:any) {
            console.error('Search error:', err);
            errorMsg.value = err?.message || 'An error occurred during search.';
        } finally {
            loading.value = false;
        }
    }
</script>
<template>
    <section class="max-w-4xl mx-auto space-y-4">
        <div class="flex items-start justify-between gap-4">
            <div>
                <h2 class="text-xl font-semibold">
                    Smart Search
                </h2>
                <p class="text-sm text-gray-600 dark:text-slate-300 mt-1">
                    Ask natural language questions. We’ll use embeddings + planning behind the scenes.
                </p>
            </div>

            <div class="flex items-center gap-2">
                <label class="text-sm text-gray-700 dark:text-slate-200">
                    Top K:
                </label>
                <input
                    type="number"
                    v-model.number="topK"
                    min="1"
                    max="20"
                     class="w-20 rounded-md border border-gray-300 dark:border-slate-600 bg-white dark:bg-slate-950 px-3 py-2 text-sm"
                />
               
            </div>
        </div>

        <InsightsSearchBox
            v-model="query"
            :loading="loading"
            @search="runSearch">
        </InsightsSearchBox>

        <div v-if="errorMsg" class="text-red-600 dark:text-red-50 text-sm">
            {{ errorMsg }}
        </div>

        <div v-if="loading" class="text-gray-600 dark:text-slate-300 text-sm">
            Searching…
        </div>
        <div v-else class="space-y-3">
            <InsightsSearchResultCard
                v-for="item in results"
                :key="item.id"
                :title="parseTitle(item)"
                :subtitle="parseSubtitle(item)"
                :score="item.score"
                :text="item.text"
            />
            <div v-if="results.length === 0" class="text-gray-600 dark:text-slate-300 text-sm">
                No results found.
            </div>

        </div>
    </section>
</template>