<script setup lang="ts">
import { error } from '#build/ui';


const {post,del} = useApi();

type RagSource = {
  id: string;
  preview: string;
  metadata: Record<string, string>;
};

type RagAskResponse = {
  answer: string;
  topK: number;
  sources: RagSource[];
};

const clearing = ref<boolean>(false);

const question = ref<string>('what product have I ordered the most?');
const topK = ref<number>(5);

const ingesting = ref<boolean>(false);
const asking = ref<boolean>(false);

const ingestResult = ref<string | null>(null);
const errorMsg = ref<string | null>(null);

const response = ref<RagAskResponse | null>(null);

useHead({
  title: 'RAG Assistant - Customer Orders App',
    meta: [
        {
        name: 'description',
        content: 'Ask questions about your customer orders using RAG.',
        },
    ],
});

async function clearRag(){
    errorMsg.value = null;
    ingestResult.value = null;
    response.value = null;
    clearing.value = true;
    try{
        await del<any>('/api/rag/clear');
        ingestResult.value = "Cleared ingested documents.";
    }
    catch(e: any){
        errorMsg.value = e?.data?.message || e?.message || "Failed to clear documents.";
    }finally{
        clearing.value = false;
    }
}

async function ingestOrders(){
    errorMsg.value = null;
    ingestResult.value = null;
    response.value = null;
    ingesting.value = true;

    try{
        const res = await post<any>('/api/rag/ingest/orders');
        console.log(res);
        ingestResult.value = `Ingested ${res.ingestedCount} orders.`;
    }catch(e: any){
        errorMsg.value = e?.data?.message || e?.message || "Failed to ingest orders.";
    }finally{
        ingesting.value = false;
    }
}

async function ask(){
    errorMsg.value = null;
    response.value = null;

    const q = question.value.trim();
    if(!q){
        errorMsg.value = 'Please enter a question.';
        return;
    }
    asking.value = true;
    try{
        const res = await post<RagAskResponse>('/api/rag/ask',{
            question: q,
            topK: topK.value
        });
        response.value = res;
    }catch(e: any){
        errorMsg.value = e?.data?.message || e?.message || "Failed to get an answer.";
    }finally{
        asking.value = false;
    }
}
</script>

<template>
    <section class="max-w-3xml mx-auto">
        <h2 class="text-xl font-semibold mb-2">Assistant</h2>
        <p class="text-sm text-gray-600 dark:text-slate-300 mb-6">
             Ingest your orders, then ask questions. The assistant will show the sources it used.
        </p>
        <!--Actions -->
        <div class = "flex flex-col sm:flex-row gap-3 mb-6">
            <button
                class="rounded-md border border-gray-300 dark:border-slate-700 px-4 py-2 text-sm hover:bg-gray-50 dark:hover:bg-slate-800 disabled:opacity-60"
                :disabled="clearing"
                @click="clearRag"
                >
                {{ clearing ? "Clearing..." : "Clear" }}
            </button>
            <button 
                class="rounded-md bg-slate-900 text-white px-4 py-2 text-sm font-medium hover:bg-slate-800 disabled:opacity-60 dark:bg-slate-100 dark:text-slate-900 dark:hover:bg-white"
                :disabled="ingesting"
                @click="ingestOrders">
                {{ ingesting ? "Ingesting..." : "Ingest Orders" }}
            </button>
            <div class="flex items-center gap-2">
                <label class="text-sm text-gray-700 dark:text-slate-200">TopK</label>
                <input
                type="number"
                v-model.number="topK"
                min="1"
                max="20"
                class="w-24 rounded-md border border-gray-300 dark:border-slate-600 bg-white dark:bg-slate-900 px-3 py-2 text-sm"/>
            </div>
        </div>

        <div v-if="ingestResult" class="mb-4 rounded-md border border-green-200 bg-green-50 text-green-800 px-4 py-3 text-sm dark:border-green-900/40 dark:bg-green-900/20 dark:text-green-200">
            {{ ingestResult }}
        </div>

        <div v-if="errorMsg" class="mb-4 rounded-md border border-red-200 bg-red-50 text-red-800 px-4 py-3 text-sm dark:border-red-900/40 dark:bg-red-900/20 dark:text-red-200">
            {{ errorMsg }}
        </div>

        <!-- Ask box -->
        <div class="rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-900 p-4 mb-6">
            <label class="block text-sm font-medium text-gray-700 dark:text-slate-200 mb-2">
                Your question
            </label>

            <textarea
                v-model="question"
                rows="3"
                class="w-full rounded-md border border-gray-300 dark:border-slate-600 bg-white dark:bg-slate-950 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="e.g., What did I order most last week?"
            ></textarea>

            <div class="mt-3 flex justify-end">
                <button
                class="rounded-md bg-blue-600 text-white px-4 py-2 text-sm font-medium hover:bg-blue-700 disabled:opacity-60"
                :disabled="asking"
                @click="ask"
                >
                {{ asking ? "Asking..." : "Ask" }}
                </button>
            </div>
        </div>


        <!-- Answer -->
        <div v-if="response" class="space-y-4">
            <div class="rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-900 p-4">
                <h3 class="text-sm font-semibold text-gray-800 dark:text-slate-100 mb-2">Answer</h3>
                <p class="text-sm text-gray-800 dark:text-slate-200 whitespace-pre-wrap">
                {{ response.answer }}
                </p>
            </div>

            <div class="rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-900 p-4">
                <h3 class="text-sm font-semibold text-gray-800 dark:text-slate-100 mb-2">
                Sources (Top {{ response.topK }})
                </h3>

                <div v-if="response.sources?.length" class="space-y-3">
                <div
                    v-for="s in response.sources"
                    :key="s.id"
                    class="rounded-md border border-gray-100 dark:border-slate-800 bg-gray-50 dark:bg-slate-950 p-3"
                >
                    <div class="text-xs font-mono text-gray-600 dark:text-slate-400 mb-1">{{ s.id }}</div>
                    <div class="text-sm text-gray-800 dark:text-slate-200 whitespace-pre-wrap">{{ s.preview }}</div>
                </div>
                </div>

                <div v-else class="text-sm text-gray-600 dark:text-slate-300">
                No sources returned. (Try ingesting orders first.)
                </div>
            </div>
        </div>


    </section>
</template>