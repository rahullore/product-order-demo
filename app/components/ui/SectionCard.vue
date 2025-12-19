<template>
  <section class="rounded-2xl bg-gray-900/50 border border-gray-800 p-5">
    <div v-if="title" class="mb-3 flex items-center justify-between">
      <h2 class="text-lg font-medium">{{ title }}</h2>
      <slot name="actions" />
    </div>

    <slot />
  </section>
</template>

<script setup lang="ts">
defineProps<{ title?: string }>();
</script>
