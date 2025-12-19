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
        <h2 class="text-lg font-semibold mb-4 text-gray-800">
            Welcome to the Customer Orders App
        </h2>
        <p class="text-sm text-gray-700 mb-4">
            The Customer Orders App is a production-grade, full-stack demo built end-to-end to showcase how modern systems are designed, delivered, and operated in the real world. This is not a tutorial project—it reflects the same architectural thinking, engineering rigor, and operational discipline I apply when building enterprise systems.
        </p>
        <p class="text-sm text-gray-700 mb-4">
            Built on .NET 9.0 and Nuxt 3, the application demonstrates how deterministic business logic and non-deterministic AI capabilities can coexist cleanly in a single system. It is fully containerized, continuously deployed via GitHub Actions, and runs live on a Linux VM behind a Caddy reverse proxy at <strong>rahullore.com</strong>.
        </p>
        <p class="text-sm text-gray-700 mb-4">
            From database migrations and API design to AI-assisted workflows and DevOps automation, every decision in this project mirrors real production constraints—performance, maintainability, observability, and ease of evolution.
        </p>

        <ul class="list-disc list-inside text-sm text-gray-800 space-y-2">
        <li>
            <strong>Production-Ready Backend Architecture</strong> – A complete .NET 9.0 API built using EF Core 9 (code-first), with clean domain boundaries and vertical slice architecture. All schema changes are managed through repeatable, versioned migrations—exactly how real systems evolve safely.
        </li>

        <li>
            <strong>End-to-End Containerized Deployment</strong> – The entire stack runs in Docker, with images published to Docker Hub and deployed onto a Linux VM. This mirrors real-world delivery pipelines used in cloud and on-prem environments.
        </li>

        <li>
            <strong>Modern DevOps & CI/CD Automation</strong> – Seamless deployments via GitHub Actions ensure every commit can be built, packaged, and released automatically, reducing human error and increasing delivery confidence.
        </li>

        <li>
            <strong>AI as a First-Class System Capability</strong> – The app demonstrates multiple AI patterns: chat completions, embeddings, and semantic search, integrated thoughtfully rather than bolted on—showing how AI augments systems instead of replacing core logic.
        </li>

        <li>
            <strong>Deterministic + Non-Deterministic Design Balance</strong> – Business-critical workflows remain deterministic and predictable, while AI is used where flexibility and intelligence add real value—reflecting responsible, production-grade AI adoption.
        </li>

        <li>
            <strong>SQL Copilot: Natural-Language Data Operations</strong> – A custom SQL Copilot enables querying, viewing, updating, and deleting data using plain English. This unlocks powerful capabilities for support teams and domain experts without deep SQL knowledge.
        </li>

        <li>
            <strong>High-Performance Full-Stack UI</strong> – Built entirely with Nuxt 3 and Tailwind CSS, the frontend is intentionally simple, fast, and responsive, focusing on usability and clarity rather than unnecessary complexity.
        </li>

        <li>
            <strong>Realistic Infrastructure Constraints</strong> – The system runs on a single Linux 22.04 VM with 8 GB RAM, hosting both the application and SQL Server—demonstrating practical resource planning and performance awareness.
        </li>

        <li>
            <strong>Security & Operational Readiness</strong> – HTTPS termination via Caddy, clean API boundaries, and controlled execution paths reflect real production hardening practices rather than demo shortcuts.
        </li>

        <li>
            <strong>Built by an Engineer Who Ships</strong> – This project reflects years of experience delivering real systems: thoughtful architecture, pragmatic trade-offs, operational awareness, and a strong focus on maintainability—not just “getting it to work.”
        </li>
        </ul>

        <footer class="mt-12 border-t border-gray-200 pt-6 text-sm text-gray-600">
  <div class="flex flex-col md:flex-row justify-between items-center gap-3">
    
    <p>
      © 2025 <strong>Rahul Lore</strong>. All rights reserved.
    </p>

    <div class="flex items-center gap-4">
      <a
        href="mailto:rahullore@gmail.com"
        class="hover:text-gray-900 transition"
      >
        rahullore@gmail.com
      </a>

      <span class="hidden md:inline text-gray-400">|</span>

      <a
        href="https://www.linkedin.com/in/rahullore"
        target="_blank"
        rel="noopener noreferrer"
        class="hover:text-gray-900 transition"
      >
        LinkedIn
      </a>

      <!-- <span class="hidden md:inline text-gray-400">|</span>

      <a
        href="https://github.com/rahullore"
        target="_blank"
        rel="noopener noreferrer"
        class="hover:text-gray-900 transition"
      >
        GitHub
      </a> -->
    </div>

  </div>

  <p class="mt-3 text-xs text-gray-500 text-center md:text-left">
    This site and its projects are built to demonstrate real-world system design, engineering practices, and AI-assisted workflows as an examples.
  </p>
</footer>


    </section>
</template>