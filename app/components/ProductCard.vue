<script setup lang="ts">
import type { PropType } from "vue";

type Product={
    id:number;
    name:string;
    description:string;
    price:number;
    stock:number;
};

const props = defineProps({
    product:{
        type: Object as PropType<Product>,
        required:true
    },
})

const emit = defineEmits<{
    (e: "select", product:Product):void
}>();

function onSelect(){
    emit("select", props.product);
}



</script>

<template>
    <article class="bg-white rounded-lg shadow p-4 border border-gray-200 dark:bg-slate-700 dark:border-slate-700">
        <h3 class="text-md font-semibold mb-1">
            <NuxtLink
                :to="`/products/${product.id}`"
                class="text-gray-800 dark:text-slate-100  hover:text-blue-600 dark:hover:text-blue-400 hover:underline ">
                {{ product.name }}
            </NuxtLink>
        </h3>
        <p class="text-sm text-gray-700 dark:text-slate-300 mb-2">{{ product.description }}</p>
        <p class="text-sm text-gray-900 dark:text-slate-100 font-medium">Price: ${{ product.price.toFixed(2) }}</p>
        <p class="text-sm text-gray-600 dark:text-slate-300">Stock: {{ product.stock }}</p>

        <button
            type="button"
            class="mt-2 w-full rounded-md bg-blue-600 px-3 py-2 text-sm font-medium text-white hover:bg-blue-700"
            @click="onSelect">
            Place Order
        </button>
    </article>
</template>