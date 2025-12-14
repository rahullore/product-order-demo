<template>
  <div class="container">
    <header>
      <h1>Product Order Demo</h1>
      <p>A showcase of Nuxt.js and .NET Core 9.0 integration</p>
    </header>
    
    <nav>
      <NuxtLink to="/products" class="nav-button">View Products</NuxtLink>
      <NuxtLink to="/orders" class="nav-button">Manage Orders</NuxtLink>
    </nav>
    
    <section class="info">
      <h2>About This Demo</h2>
      <p>This application demonstrates:</p>
      <ul>
        <li>Nuxt.js 4 frontend with TypeScript</li>
        <li>.NET Core 9.0 Web API backend</li>
        <li>RESTful API integration</li>
        <li>Product catalog management</li>
        <li>Order processing system</li>
      </ul>
    </section>
  </div>
</template>

<script setup lang="ts">
useHead({
  title: 'Product Order Demo - Home'
})
</script>

<style scoped>
.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
  font-family: Arial, sans-serif;
}

header {
  text-align: center;
  margin-bottom: 3rem;
}

h1 {
  color: #2c3e50;
  font-size: 2.5rem;
  margin-bottom: 0.5rem;
}

header p {
  color: #7f8c8d;
  font-size: 1.2rem;
}

nav {
  display: flex;
  justify-content: center;
  gap: 2rem;
  margin-bottom: 3rem;
}

.nav-button {
  display: inline-block;
  padding: 1rem 2rem;
  background: #3498db;
  color: white;
  text-decoration: none;
  border-radius: 8px;
  font-weight: bold;
  transition: background 0.3s;
}

.nav-button:hover {
  background: #2980b9;
}

.info {
  background: #f8f9fa;
  padding: 2rem;
  border-radius: 8px;
}

.info h2 {
  color: #2c3e50;
  margin-bottom: 1rem;
}

.info ul {
  list-style-position: inside;
  line-height: 1.8;
}

.info li {
  color: #34495e;
}
</style>
