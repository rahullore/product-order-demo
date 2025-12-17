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
            Welcome to the Customer Orders App—a full-stack demo showcasing modern .NET 9.0 development practices I've mastered while building it from scratch. Deployed live at rahullore.com with Caddy reverse proxy and GitHub Actions CI/CD, it integrates advanced AI capabilities with a robust frontend.
        </p>
        <ul class="list-disc list-inside text-sm text-gray-800 space-y-1">
            <li><strong>Backend Excellence</strong>: Real .NET 9.0 API handling customer orders, product loading from REST endpoints, and in-memory order persistence—production-ready with vertical slice architecture.</li>
            <li><strong>AI-Powered Features</strong>: Basic chat completions via Perplexity API plus advanced embeddings for semantic scoring, stored in a vector DB for intelligent RAG retrieval.</li>
            <li><strong>Frontend Mastery</strong>: Nuxt.js with advanced routing, built-in API routes using Fetch, and Tailwind CSS for responsive, modern UI.</li>
            <li><strong>DevOps Pipeline</strong>: Automated CI/CD via GitHub Actions, containerized deployment, and Caddy proxy for secure HTTPS on custom domain rahullore.com.</li>
            <li>Place orders, chat with AI, explore semantic search—use navigation above to dive in!</li>
        </ul>

    </section>
</template>