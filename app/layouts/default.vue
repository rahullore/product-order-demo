<script setup>

const route = useRoute();
const {orders} = useOrders();
</script>
<template>
    <div header class=min-h-screen bg-gray-100 text-gray-900>
        <header class="bg-white shadow">
            <div class="max-w-4xl mx-auto px-4 py-4 flex item-center justify-between">
                <h1 class="text-xl font-semibold text-gray-800">
                    Customer Orders App
                </h1>
                <nav class="space-x-4 text-sm">
                    <NuxtLink to="/" class="hover:text-blue-600" :class="{'text-blue-600 font-semibold' : route.path==='/' }">Home</NuxtLink>
                    <NuxtLink to="/products" class="hover:text-blue-600" :class="{'text-blue-600 font-semibold' : route.path==='/products' }">Products</NuxtLink>
                    <NuxtLink to="/orders" class="hover:text-blue-600 relative" :class="{'text-blue-600 font-semibold' : route.path==='/orders' }">Orders
                        <span v-if="orders.length>0"
                            class="ml-1 inline-flex items-center justify-center text-xs bg-red-500 text-white rounded-full px-2">
                            {{ orders.length }}
                        </span>
                    </NuxtLink>
                </nav>
            </div>
        </header>
        <main class="max-w-4xl mx-auto px-4 py-8">
            <slot />
        </main>
        <ToastContainer />
    </div>
</template>