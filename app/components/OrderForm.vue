<script setup lang="ts">
import type { PropType } from "vue";
import { useOrders } from "~/composables/useOrders";

const props = defineProps({
    product:{
        type: Object as PropType<{
            id:number;
            name:string;
            price:number;
        }>,
        required:true
    },
})

const emit = defineEmits(["close"]);

const quantity = ref(1);

const {addOrder} = useOrders();

function submit(){
    addOrder({
        productId: props.product.id,
        productName: props.product.name,
        quantity: quantity.value,
        price: props.product.price,
    })

    console.log("Order submitted");
    emit("close");
}
</script>

<template>
    <div class="fixed inset-0 bg-block/40 flex items-center justify-center">
        <div class="bg-white rounded-lg shadow p-6 w-80">
            <h3 class ="text-lg font-semibold mb-4">
                Place Order
            </h3>
            <p class="text-sm mb-2">
                Product: <strong>{{ product.name }}</strong>
            </p>
            <p class="text-sm mb-4">
                Price: <strong>${{ product.price.toFixed(2) }}</strong>
            </p>
            <label class="block text-sm mb-1">
                Quantity</label>
            <input
                type="number"
                v-model.number="quantity"
                min="1"
                class="w-full border border-gray-300 rounded-md px-3 py-2 mb-4"
            />
            <div class="flex gap-2">
                <button
                    type="button"
                    @click="submit"
                    class="w-full rounded-md bg-green-600 px-3 py-2 text-sm font-medium text-white hover:bg-green-700">
                    Submit Order
                </button>
                <button
                    type="button"
                    @click="$emit('close')"
                    class="w-full rounded-md bg-gray-300 px-3 py-2 text-sm font-medium text-gray-800 hover:bg-gray-400">
                    Cancel
                </button>
            </div>
        </div>
    </div>
    

   </template>
