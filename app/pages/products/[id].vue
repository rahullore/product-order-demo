<script setup lang="ts">
const route = useRoute();

console.log("DETAIL PAGE MOUNTED. id =", route.params.id)

type Product ={
    id:number;
    name:string;
    description:string;
    price:number;
    stock:number; 
}

// Get numeric id from URL like /products/1
const productId = computed(() => {
  const raw = route.params.id
  const value = Array.isArray(raw) ? raw[0] : raw
  const n = Number(value)
  return Number.isFinite(n) ? n : -1
})

const {data: products,pending,error}=useFetch<Product[]>("/api/products");

console.log("All products:", products);

const product = computed<Product | null>(()=>{
    return products.value?.find(p=>p.id===productId.value) || null;
})

const pageTitle = computed(()=>{
    return product.value ? `${product.value.name} - Customer Orders App` : 'Product Not Found - Customer Orders App';
})

useHead(()=>({
    title: pageTitle.value,
}))

    
</script>

<template>
    <section>
        <NuxtLink to="/products" class="text-sm text-blue-600 hover:underline"><- Back to Products</NuxtLink>

        <div class="mt-4">
            <div v-if="pending" class="text-sm text-gray-600">
                Loading product details...
            </div>
            <div v-else-if="error" class="text-sm text-red-600">
                Error loading product details: {{ error.message }}
            </div>
            <div v-else-if="!product" class="text-sm text-gray-600">
                Product not found.
            </div>
            <div
                v-else
                class="bg-white rounded-lg shadow p-6 border border-gray-100">
                <h2 class ="text-xl font-semibold text-gray-900 mb-2">{{ product.name }}</h2>
                <p class="text-sm text-gray-700 mb-4">{{ product.description }}</p>
                <div class="flex item-center justify-between mb-2">
                    <span class="text-lg font-semibold text-gray-900">{{ product.price.toFixed(2) }} </span>
                    <span class="text-sm text-gray-600">Stock: {{ product.stock }}</span>
                </div>
                <p class="text-xs text-gray-500">Product ID: {{ product.id }}</p>
            </div>
        </div>
    </section>
</template>