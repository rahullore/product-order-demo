<script setup lang="ts">
import { useOrders } from "~/composables/useOrders";
const { orders, clearOrders } = useOrders();
useHead({
    title: 'Order List - Customer Orders App',
    meta: [
        { name: 'description', content: 'View the list of customer orders placed for products.' }
    ],
})

const showClearConfirm = ref(false);

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
</script>

<template>
    <ClientOnly>
        <section>
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
                    class="bg-white rounded-lg shadow p-4 border border-gray-100">
                    <h3 class="text-md font-semibold text-gray-800">{{ order.productName }}</h3>
                    <p class="text-sm text-gray-700 mb-2">Quantity: {{ order.quantity }}</p>
                    <p class="text-sm text-gray-900 font-medium">Total Price: ${{ (order.price *
                        order.quantity).toFixed(2) }}</p>
                </article>
            </div>

            <pre class="text-xs bg-gray-900 text-green-300 p-3 rounded">
                {{ JSON.stringify(orders, null, 2) }}
            </pre>

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