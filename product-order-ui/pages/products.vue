<template>
  <div class="container">
    <header>
      <h1>Product Catalog</h1>
      <NuxtLink to="/" class="back-link">← Back to Home</NuxtLink>
    </header>

    <div v-if="pending" class="loading">Loading products...</div>
    <div v-else-if="error" class="error">Error loading products: {{ error.message }}</div>
    
    <div v-else class="products-grid">
      <div v-for="product in products" :key="product.id" class="product-card">
        <img :src="product.imageUrl" :alt="product.name" class="product-image" />
        <h3>{{ product.name }}</h3>
        <p class="description">{{ product.description }}</p>
        <div class="product-info">
          <span class="price">${{ product.price.toFixed(2) }}</span>
          <span class="stock" :class="{ 'low-stock': product.stock < 20 }">
            Stock: {{ product.stock }}
          </span>
        </div>
        <button @click="addToCart(product)" class="add-button">Add to Cart</button>
      </div>
    </div>

    <div v-if="cart.length > 0" class="cart">
      <h2>Shopping Cart</h2>
      <div v-for="item in cart" :key="item.productId" class="cart-item">
        <span>{{ item.productName }} x {{ item.quantity }}</span>
        <span>${{ (item.price * item.quantity).toFixed(2) }}</span>
        <button @click="removeFromCart(item.productId)" class="remove-btn">×</button>
      </div>
      <div class="cart-total">
        <strong>Total: ${{ cartTotal.toFixed(2) }}</strong>
      </div>
      <button @click="proceedToCheckout" class="checkout-button">Proceed to Checkout</button>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Product {
  id: number
  name: string
  description: string
  price: number
  stock: number
  imageUrl: string
}

interface CartItem {
  productId: number
  productName: string
  quantity: number
  price: number
}

useHead({
  title: 'Products - Product Order Demo'
})

const config = useRuntimeConfig()
const router = useRouter()

const { data: products, pending, error } = await useFetch<Product[]>(`${config.public.apiBase}/api/products`)

const cart = ref<CartItem[]>([])

const cartTotal = computed(() => {
  return cart.value.reduce((sum, item) => sum + (item.price * item.quantity), 0)
})

const addToCart = (product: Product) => {
  const existingItem = cart.value.find(item => item.productId === product.id)
  if (existingItem) {
    existingItem.quantity++
  } else {
    cart.value.push({
      productId: product.id,
      productName: product.name,
      quantity: 1,
      price: product.price
    })
  }
}

const removeFromCart = (productId: number) => {
  const index = cart.value.findIndex(item => item.productId === productId)
  if (index > -1) {
    cart.value.splice(index, 1)
  }
}

const proceedToCheckout = () => {
  // Store cart in session storage for checkout
  if (process.client) {
    sessionStorage.setItem('cart', JSON.stringify(cart.value))
    router.push('/checkout')
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

.loading, .error {
  text-align: center;
  padding: 2rem;
  font-size: 1.2rem;
}

.error {
  color: #e74c3c;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 2rem;
  margin-bottom: 2rem;
}

.product-card {
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 1rem;
  transition: box-shadow 0.3s;
}

.product-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.product-image {
  width: 100%;
  height: 150px;
  object-fit: cover;
  border-radius: 4px;
  margin-bottom: 1rem;
}

h3 {
  color: #2c3e50;
  margin-bottom: 0.5rem;
  font-size: 1.2rem;
}

.description {
  color: #7f8c8d;
  margin-bottom: 1rem;
  font-size: 0.9rem;
}

.product-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.price {
  font-size: 1.5rem;
  color: #27ae60;
  font-weight: bold;
}

.stock {
  color: #27ae60;
  font-size: 0.9rem;
}

.stock.low-stock {
  color: #e67e22;
}

.add-button {
  width: 100%;
  padding: 0.75rem;
  background: #3498db;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
  transition: background 0.3s;
}

.add-button:hover {
  background: #2980b9;
}

.cart {
  background: #f8f9fa;
  padding: 2rem;
  border-radius: 8px;
  margin-top: 2rem;
}

.cart h2 {
  color: #2c3e50;
  margin-bottom: 1rem;
}

.cart-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background: white;
  margin-bottom: 0.5rem;
  border-radius: 4px;
}

.remove-btn {
  background: #e74c3c;
  color: white;
  border: none;
  border-radius: 50%;
  width: 25px;
  height: 25px;
  cursor: pointer;
  font-size: 1.2rem;
  line-height: 1;
}

.cart-total {
  text-align: right;
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 2px solid #ddd;
  font-size: 1.3rem;
}

.checkout-button {
  width: 100%;
  padding: 1rem;
  background: #27ae60;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
  font-size: 1.1rem;
  margin-top: 1rem;
  transition: background 0.3s;
}

.checkout-button:hover {
  background: #229954;
}
</style>
