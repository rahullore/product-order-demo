// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  modules: ['@nuxt/ui'],
  ssr: true,
  css: ['~/assets/css/main.css'] // ⬅️ tell Nuxt to load our global CSS
})