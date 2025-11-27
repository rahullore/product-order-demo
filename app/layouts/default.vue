<template>
    <div header class=min-h-screen bg-gray-100 text-gray-900>
        <header class="bg-white shadow">
            <div class="max-w-4xl mx-auto px-4 py-4 flex item-center justify-between">
                <h1 class="text-xl font-semibold text-gray-800">
                    Customer Orders App
                </h1>
                <nav class="space-x-4 text-sm">
                    <NuxtLink to="/" class="hover:text-blue-600">Home</NuxtLink>
                    <NuxtLink to="/products" class="hover:text-blue-600">Products</NuxtLink>
                    <NuxtLink to="/orders" class="hover:text-blue-600">Orders</NuxtLink>
                </nav>
            </div>
        </header>
        <main class="max-w-4xl mx-auto px-4 py-8">
            <slot />
        </main>
    </div>
</template>