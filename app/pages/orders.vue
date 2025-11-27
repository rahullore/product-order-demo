<script setup lang="ts">
import { useOrders } from "~/composables/useOrders";
const {orders} = useOrders();
useHead({   
    title: 'Order List - Customer Orders App',
        meta: [
                { name: 'description', content: 'View the list of customer orders placed for products.' }
        ],
})
</script>

<template>
    <ClientOnly>
        <section>
            <h2 class="text-lg font-semibold mb-4">Order List</h2>

            <div v-if="orders.length === 0" class="text-sm text-gray-600">
                No orders have been placed yet.
            </div>
            <div v-else class="space-y-4">
                <article v-for="(order, index) in orders" :key="index" class="bg-white rounded-lg shadow p-4 border border-gray-100">
                    <h3 class="text-md font-semibold text-gray-800">{{ order.productName }}</h3>
                    <p class="text-sm text-gray-700 mb-2">Quantity: {{ order.quantity }}</p>
                    <p class="text-sm text-gray-900 font-medium">Total Price: ${{ (order.price * order.quantity).toFixed(2) }}</p>
                </article>
            </div>

            <pre class="text-xs bg-gray-900 text-green-300 p-3 rounded">
                {{ JSON.stringify(orders, null, 2) }}
            </pre>
        </section>
    </ClientOnly>
</template>