<script setup lang="ts">


type Product = {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
};

const productApiUrl = 'http://localhost:5178/api/products';

const {data: products,pending,error}=useFetch<Product[]>(productApiUrl);

useHead({   
  title: 'Product List - Customer Orders App',
    meta: [
        { name: 'description', content: 'Browse our list of products available for customer orders.' }
    ],
})

const selectedProduct = ref<null | {
  id: number;
  name: string;
  price: number;
}>(null);

function openOrderForm(product: Product) {
  selectedProduct.value = {
    id: product.id,
    name: product.name,
    price: product.price,
  };
}
function closeOrderForm() {
  selectedProduct.value = null;
}

const searchTerm = ref('');

const filteredProducts = computed(() => {
    if(!products.value) return [];
    const term = searchTerm.value.toLowerCase();
    if(!term) return products.value;

    return products.value.filter(p=>
        p.name.toLowerCase().includes(term) ||
        p.description.toLowerCase().includes(term)
    );  
});

</script>

<template>
    <section>
        <h2 class="text-lg font-semibold mb-4">Product List</h2>
        <div class="mb-4">
            <label class="block text-sm text-gray-700 mb-1">
                Search Products:
                <input
                    type="text"
                    v-model="searchTerm"
                    placeholder="Enter product name or description"
                    class="w-full rounded-md border border-gray-300 dark:border-slate-600 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                    />
            </label>
        </div>

        <div v-if="pending" class="text-sm text-gray-600 dark:text-slate-300">Loading products...</div>
        <div v-else-if="error" class="text-sm text-red-600 dark:text-red-400">Error loading products: {{ error.message }}</div>
        <!--<div v-else class="space-y-4">
            <article v-for="product in products" :key="product.id" class="bg-white rounded-lg shadow p-4 border border-gray-100">
                <h3 class="text-md font-semibold text-gray-800">{{ product.name }}</h3>
                <p class="text-sm text-gray-700 mb-2">{{ product.description }}</p>
                <p class="text-sm text-gray-900 font-medium">Price: ${{ product.price.toFixed(2) }}</p>
                <p class="text-sm text-gray-600">Stock: {{ product.stock }}</p>

                <button
                    type="button"
                    class="mt-2 w-full rounded-md bg-blue-600 px-3 py-2 text-sm font-medium text-white hover:bg-blue-700"
                    @click="openOrderForm(product)">
                    Place Order
                </button>
            </article>
        </div>-->
        <div v-else class="space-y-4">
            <ProductCard
                v-for="product in filteredProducts"
                :key="product.id"
                :product="product"
                @select="openOrderForm"
            />
        </div>
        <OrderForm
            v-if="selectedProduct"
            :product="selectedProduct"
            @close="closeOrderForm"
        />
        

    </section>
</template>
