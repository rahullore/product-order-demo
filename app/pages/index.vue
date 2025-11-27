<script setup lang="ts">
useHead({   
  title: 'Customer Orders App',
    meta: [
        { name: 'description', content: 'A simple app to manage customer orders.' }
    ],
})
</script>

<template>
    <section class="bg-white rounded-lg shadow p-6">
        <h2 class ="text-lg font-semibold mb-4 text-gray-800">Welcome to the Customer Orders App</h2>
        <p class="text-sm text-gray-700 mb-4">
            This application allows you to manage customer orders efficiently. Use the navigation links above to get started.
        </p>
        <ul class="list disc list-inside text-sm text-gray-800 space-y-1">
            <li>Load a list of products from a dummy REST API endpoint.</li>
            <li>Plcase customer order for a selected product.</li>
            <li>Show the list of orders stored in memory.</li>
        </ul>
    </section>
</template>