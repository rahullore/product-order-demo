<script setup lang="ts">

defineProps<{
    title: string
    subtitle?: string
    score?: number
    text?: string
}>()

const fmtScore = (n?:number) =>{
    typeof n==='number' ? n.toFixed(3) : ''
}
</script>
<template>

    <article class="rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-900 p-4">
        <div class="flex items-start justify-between gap-3">
            <div>
                <h3 class="text-sm font-semibold text-gray-900 dark:text-slate-100">
                    {{ title }}
                </h3>
                <p v-if="subtitle" class="text-xs text-gray-600 dark:text-slate-300 mt-1">
                    {{ subtitle }}
                </p>
            </div>

            <div v-if="score != undefined" class="text-xs font-mono text-gray-500 dark:text-slate-400">
                {{ fmtScore(score) }}
            </div>
        </div>

        <p v-if="text" class="mt-3 text-sm text-gray-800 dark:text-slate-200 whitespace-pre-wrap">
            {{ text }}
        </p>

    </article>
</template>