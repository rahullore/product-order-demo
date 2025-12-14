<template>
  <div class="container">
    <header>
      <h1>Checkout</h1>
      <NuxtLink to="/products" class="back-link">← Back to Products</NuxtLink>
    </header>

    <div v-if="!cart || cart.length === 0" class="empty">
      <p>Your cart is empty.</p>
      <NuxtLink to="/products" class="shop-link">Go Shopping</NuxtLink>
    </div>

    <div v-else class="checkout-content">
      <div class="order-summary">
        <h2>Order Summary</h2>
        <div v-for="item in cart" :key="item.productId" class="order-item">
          <span>{{ item.productName }} x {{ item.quantity }}</span>
          <span>${{ (item.price * item.quantity).toFixed(2) }}</span>
        </div>
        <div class="order-total">
          <strong>Total: ${{ orderTotal.toFixed(2) }}</strong>
        </div>
      </div>

      <form @submit.prevent="placeOrder" class="checkout-form">
        <h2>Customer Information</h2>
        
        <div class="form-group">
          <label for="name">Full Name *</label>
          <input 
            v-model="customerName" 
            type="text" 
            id="name" 
            required 
            placeholder="John Doe"
          />
        </div>

        <div class="form-group">
          <label for="email">Email *</label>
          <input 
            v-model="customerEmail" 
            type="email" 
            id="email" 
            required 
            placeholder="john@example.com"
          />
        </div>

        <div v-if="error" class="error-message">{{ error }}</div>
        <div v-if="success" class="success-message">
          Order placed successfully! Order ID: {{ orderId }}
          <NuxtLink to="/orders" class="view-orders-link">View Orders</NuxtLink>
        </div>

        <button type="submit" :disabled="submitting" class="submit-button">
          {{ submitting ? 'Placing Order...' : 'Place Order' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
interface CartItem {
  productId: number
  productName: string
  quantity: number
  price: number
}

useHead({
  title: 'Checkout - Product Order Demo'
})

const config = useRuntimeConfig()
const cart = ref<CartItem[]>([])
const customerName = ref('')
const customerEmail = ref('')
const submitting = ref(false)
const error = ref('')
const success = ref(false)
const orderId = ref<number | null>(null)

onMounted(() => {
  if (process.client) {
    const cartData = sessionStorage.getItem('cart')
    if (cartData) {
      cart.value = JSON.parse(cartData)
    }
  }
})

const orderTotal = computed(() => {
  return cart.value.reduce((sum, item) => sum + (item.price * item.quantity), 0)
})

const placeOrder = async () => {
  submitting.value = true
  error.value = ''
  
  try {
    const orderData = {
      customerName: customerName.value,
      customerEmail: customerEmail.value,
      items: cart.value.map(item => ({
        productId: item.productId,
        productName: item.productName,
        quantity: item.quantity,
        price: item.price
      }))
    }

    interface OrderResponse {
      id: number
      customerName: string
      customerEmail?: string
      orderDate: string
      status: number
      items: CartItem[]
      totalAmount: number
    }

    const response = await $fetch<OrderResponse>(`${config.public.apiBase}/api/orders`, {
      method: 'POST',
      body: orderData
    })

    success.value = true
    orderId.value = response.id
    
    // Clear cart
    cart.value = []
    if (process.client) {
      sessionStorage.removeItem('cart')
    }
  } catch (e) {
    const errorMessage = e instanceof Error ? e.message : 'Failed to place order. Please try again.'
    error.value = errorMessage
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
  font-family: Arial, sans-serif;
}

header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

h1 {
  color: #2c3e50;
  font-size: 2rem;
}

.back-link {
  color: #3498db;
  text-decoration: none;
  font-weight: bold;
}

.back-link:hover {
  text-decoration: underline;
}

.empty {
  text-align: center;
  padding: 3rem;
}

.empty p {
  font-size: 1.2rem;
  color: #7f8c8d;
  margin-bottom: 1rem;
}

.shop-link {
  display: inline-block;
  padding: 1rem 2rem;
  background: #3498db;
  color: white;
  text-decoration: none;
  border-radius: 4px;
  font-weight: bold;
}

.checkout-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
}

@media (max-width: 768px) {
  .checkout-content {
    grid-template-columns: 1fr;
  }
}

.order-summary {
  background: #f8f9fa;
  padding: 2rem;
  border-radius: 8px;
  height: fit-content;
}

.order-summary h2 {
  color: #2c3e50;
  margin-bottom: 1rem;
}

.order-item {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem;
  background: white;
  margin-bottom: 0.5rem;
  border-radius: 4px;
}

.order-total {
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 2px solid #ddd;
  font-size: 1.3rem;
  text-align: right;
}

.checkout-form {
  background: #f8f9fa;
  padding: 2rem;
  border-radius: 8px;
}

.checkout-form h2 {
  color: #2c3e50;
  margin-bottom: 1.5rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  color: #2c3e50;
  font-weight: bold;
}

input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

input:focus {
  outline: none;
  border-color: #3498db;
}

.error-message {
  padding: 1rem;
  background: #fee;
  color: #e74c3c;
  border-radius: 4px;
  margin-bottom: 1rem;
}

.success-message {
  padding: 1rem;
  background: #d4edda;
  color: #155724;
  border-radius: 4px;
  margin-bottom: 1rem;
}

.view-orders-link {
  display: block;
  margin-top: 0.5rem;
  color: #155724;
  font-weight: bold;
}

.submit-button {
  width: 100%;
  padding: 1rem;
  background: #27ae60;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
  font-size: 1.1rem;
  transition: background 0.3s;
}

.submit-button:hover:not(:disabled) {
  background: #229954;
}

.submit-button:disabled {
  background: #95a5a6;
  cursor: not-allowed;
}
</style>
