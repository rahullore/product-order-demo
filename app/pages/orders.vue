<script setup lang="ts">
import { useOrders } from "~/composables/useOrders";
const { orders, clearOrders, loadOrders,loading } = useOrders();
const {post} = useApi();

useHead({
    title: 'Order List - Customer Orders App',
    meta: [
        { name: 'description', content: 'View the list of customer orders placed for products.' }
    ],
})

onMounted(() => {
    loadOrders();
});

type OrderAskResponse ={
    answer:string,
    ordersIncluded: number,
    usedOrderIds: number[]
}

const showClearConfirm = ref(false);
const aiQuestion = ref('Summerize my last orders and total spend')
const aiAnswer = ref<string>('')
const aiMeta = ref<{orderIncluded: number, usedOrderIds: number[]} | null>(null)
const aiLoading = ref(false)
const aiError = ref<string>('')

function requestClear() {
    if (orders.value.length === 0) return;
    showClearConfirm.value = true;
}

function confirmClear() {
    clearOrders();
    showClearConfirm.value = false;
}

function cancelClear() {
    showClearConfirm.value = false;
}   

async function askAiAboutOrders(){
    aiLoading.value = true
    aiError.value = ''
    aiAnswer.value = ''
    aiMeta.value = null
    try {
        const res = await post<OrderAskResponse>('/api/ai/orders/ask', {
            question: aiQuestion.value,
            orders: orders.value
        })

        aiAnswer.value = res.answer
        aiMeta.value = {
            orderIncluded: res.ordersIncluded,
            usedOrderIds: res.usedOrderIds
        }
    } catch (error: any) {
        aiError.value = error.message || 'An error occurred while asking AI about orders.'
    } finally {
        aiLoading.value = false
    }
}
</script>

<template>
    <ClientOnly>
        <section class="mb-6 bg-white dark:bg-slate-800 rounded-lg shadow p-4 border border-gray-200 dark:border-slate-700">
            <h3 class="text-md font-semibold mb-2 dark:text-slate-100">Ask AI about your orders</h3>
            <label class ="block text-sm text-gray-700 dark:text-slate-200 mb-1">
                Your Question:
            </label>
            <textarea
                v-model="aiQuestion"
                rows="3"
                class="w-full rounded-md border border-gray-300 dark:border-slate-600
                bg-white dark:bg-slate-900
                text-gray-900 dark:text-slate-100
                px-3 py-2 text-sm
                placeholder:text-gray-400 dark:placeholder:text-slate-500
                focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="E.g., Summarize my last orders and total spend"/>
            <div class="mt-3 flex gap-2">
                <button
                    type="button"
                    class="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-60"
                    :disabled="aiLoading || !aiQuestion.trim()"
                    @click="askAiAboutOrders">
                    {{ aiLoading ? 'Asking...' : 'Ask AI' }}
                </button>   
                <button
                    type="button"
                    class="rounded-md border border-gray-300 dark:border-slate-600 px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-slate-700"
                    :disabled="aiLoading"
                    @click="() => { aiQuestion=''; aiAnswer=''; aiError=''; aiMeta=null }">
                    Clear
                </button>
            </div>
            <p v-if="aiError" class="mt-3 text-sm text-red-600">{{ aiError }}</p>

            <div v-if="aiAnswer" class="mt-4">
                <div class="text-xs text-gray-500 dark:text-slate-400 mb-2" v-if="aiMeta">
                    Used {{ aiMeta.orderIncluded }} orders (IDs: {{ aiMeta.usedOrderIds.join(', ') }})
                </div>
                <div class="rounded-md bg-gray-50 dark:bg-slate-900 border border-gray-200 dark:border-slate-700 p-3 text-sm text-gray-900 dark:text-slate-100 whitespace-pre-wrap">

                    {{ aiAnswer }}
                </div>
            </div>

            <h2 class="text-lg font-semibold mb-4">Order List</h2>
            <button
                type="button"
                @click="requestClear"
                 class="bg-white dark:bg-slate-800 rounded-lg shadow p-4 border border-gray-100 dark:border-slate-700">
                Clear All Orders
            </button>

            <div v-if="orders.length === 0"
                class="bg-white border border-dashed border-gray-300 rounded-lg p-8 text-center text-gray-500">
                <p class="text-lg font-medium mb-2">No orders have been placed yet.</p>
                <p class="text-sm mb-4">Go to the Products page and place your first order.</p>
                <NuxtLink to="/products"
                    class="inline-block bg-blue-600 text-white text-sm px-4 py-2 rounded hover:bg-blue-700">
                    Browse Products
                </NuxtLink>
            </div>
            <div v-else class="space-y-4">
                <article v-for="(order, index) in orders" :key="index"
                    class="bg-white rounded-lg shadow p-4 border border-gray-200 dark:bg-slate-700 dark:border-slate-700">
                    <h3 class="text-sm text-gray-900 dark:text-slate-100 font-medium ">{{ order.productName }}</h3>
                    <p class="text-sm text-gray-700 dark:text-slate-300 mb-2">Quantity: {{ order.quantity }}</p>
                    <p class="text-sm text-gray-900 dark:text-slate-100 font-medium">Total Price: ${{ (order.price *
                        order.quantity).toFixed(2) }}</p>
                </article>
            </div>

            <!-- <pre class="text-xs bg-gray-900 text-green-300 p-3 rounded">
                {{ JSON.stringify(orders, null, 2) }}
            </pre>-->

            <ConfirmDialog
            v-if="showClearConfirm"
            title="Clear All Orders"
            message="Are you sure you want to clear all orders? This action cannot be undone."
            confirmLabel="Yes, Clear All"
            cancelLabel="Cancel"
            @confirm="confirmClear"
            @cancel="cancelClear"/>
        </section>
        
    </ClientOnly>
</template>