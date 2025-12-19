<script setup>

const route = useRoute();
const {orders} = useOrders();
const { theme, toggleTheme } = useTheme();
</script>
<template>
    <div class="min-h-screen bg-gray-100 text-gray-900 dark:bg-slate-900 dark:text-slate-100">
        <header class="bg-white shadow dark:bg-slate-800">
            <div class="max-w-4xl mx-auto px-4 py-4 flex item-center justify-between">
                <h1 class="text-xl font-semibold text-gray-800 dark:text-slate-200">
                    Customer Orders App
                </h1>
                <nav class="space-x-4 text-sm">
                    <NuxtLink to="/" class="hover:text-blue-600 dark:hover:text-blue-400" :class="{'text-blue-600 font-semibold' : route.path==='/' }">Home</NuxtLink>
                    <NuxtLink to="/products" class="hover:text-blue-600 dark:hover:text-blue-400" :class="{'text-blue-600 font-semibold' : route.path==='/products' }">Products</NuxtLink>
                    <NuxtLink to="/orders" class="hover:text-blue-600 dark:hover:text-blue-400 relative" :class="{'text-blue-600 font-semibold' : route.path==='/orders' }">Orders
                        <span v-if="orders.length>0"
                            class="ml-1 inline-flex items-center justify-center text-xs bg-red-500 text-white rounded-full px-2">
                            {{ orders.length }}
                        </span>
                    </NuxtLink>
                    <NuxtLink to="/assistant" class="hover:text-blue-600 dark:hover:text-blue-400" :class="{'text-blue-600 font-semibold' : route.path==='/assistant' }">Assistant</NuxtLink>
                    <NuxtLink to="/insights" class="hover:text-blue-600 dark:hover:text-blue-400" :class="{'text-blue-600 font-semibold' : route.path==='/insights' }">Insights</NuxtLink>
                    <NuxtLink to="/insight-search" class="hover:text-blue-600 dark:hover:text-blue-400" :class="{'text-blue-600 font-semibold' : route.path==='/insight-search' }">Smart Search</NuxtLink>
                    <NuxtLink to="/sqlcopilot" class="hover:text-blue-600 dark:hover:text-blue-400" :class="{'text-blue-600 font-semibold' : route.path==='/sqlcopilot' }">SQL Copilot</NuxtLink>


                </nav>
                <button
                    type="button"
                    @click="toggleTheme"
                    class="ml-4 p-2 rounded-md bg-gray-200 dark:bg-slate-700 hover:bg-gray-300 dark:hover:bg-slate-600">
                    <span v-if="theme === 'light'">🌙</span>
                    <span v-else>☀️</span>
                </button>
            </div>
        </header>
        <main class="max-w-4xl mx-auto px-4 py-8">
            <slot />
        </main>
        <ToastContainer />
    </div>
</template>