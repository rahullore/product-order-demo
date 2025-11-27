<script setup lang="ts">
import OrderForm from '~/components/OrderForm.vue';

type Product = {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
};

const {data: products,pending,error}=useFetch<Product[]>('/api/products');

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

</script>

<template>
    <section>
        <h2 class="text-lg font-semibold mb-4">Product List</h2>

        <div v-if="pending" class="text-sm text-gray-600">Loading products...</div>
        <div v-else-if="error" class="text-sm text-red-600">Error loading products: {{ error.message }}</div>
        <div v-else class="space-y-4">
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
        </div>
        <OrderForm
            v-if="selectedProduct"
            :product="selectedProduct"
            @close="closeOrderForm"
        />
        

    </section>
</template>
