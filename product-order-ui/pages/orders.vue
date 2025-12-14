<template>
  <div class="container">
    <header>
      <h1>Order Management</h1>
      <NuxtLink to="/" class="back-link">← Back to Home</NuxtLink>
    </header>

    <div v-if="pending" class="loading">Loading orders...</div>
    <div v-else-if="error" class="error">Error loading orders: {{ error.message }}</div>
    
    <div v-else-if="!orders || orders.length === 0" class="empty">
      <p>No orders yet.</p>
      <NuxtLink to="/products" class="shop-link">Start Shopping</NuxtLink>
    </div>

    <div v-else class="orders-list">
      <div v-for="order in orders" :key="order.id" class="order-card">
        <div class="order-header">
          <h3>Order #{{ order.id }}</h3>
          <span class="status" :class="`status-${getStatusName(order.status).toLowerCase()}`">
            {{ getStatusName(order.status) }}
          </span>
        </div>
        
        <div class="order-info">
          <p><strong>Customer:</strong> {{ order.customerName }}</p>
          <p v-if="order.customerEmail"><strong>Email:</strong> {{ order.customerEmail }}</p>
          <p><strong>Date:</strong> {{ formatDate(order.orderDate) }}</p>
        </div>

        <div class="order-items">
          <h4>Items:</h4>
          <div v-for="item in order.items" :key="item.id" class="order-item">
            <span>{{ item.productName }} x {{ item.quantity }}</span>
            <span>${{ (item.price * item.quantity).toFixed(2) }}</span>
          </div>
        </div>

        <div class="order-total">
          <strong>Total: ${{ order.totalAmount.toFixed(2) }}</strong>
        </div>

        <div class="order-actions">
          <select 
            :value="getStatusName(order.status)" 
            @change="updateOrderStatus(order.id, ($event.target as HTMLSelectElement).value)"
            class="status-select"
          >
            <option value="Pending">Pending</option>
            <option value="Processing">Processing</option>
            <option value="Shipped">Shipped</option>
            <option value="Delivered">Delivered</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface OrderItem {
  id: number
  productId: number
  productName: string
  quantity: number
  price: number
}

interface Order {
  id: number
  customerName: string
  customerEmail?: string
  orderDate: string
  status: number | string
  items: OrderItem[]
  totalAmount: number
}

useHead({
  title: 'Orders - Product Order Demo'
})

const config = useRuntimeConfig()

const { data: orders, pending, error, refresh } = await useFetch<Order[]>(`${config.public.apiBase}/api/orders`)

const statusNames = ['Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled']

const getStatusName = (status: number | string): string => {
  if (typeof status === 'string') return status
  return statusNames[status] || 'Unknown'
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString() + ' ' + date.toLocaleTimeString()
}

const updateOrderStatus = async (orderId: number, status: number | string) => {
  try {
    // Convert string status name to number if needed
    let statusValue = status
    if (typeof status === 'string') {
      statusValue = statusNames.indexOf(status)
    }
    
    await $fetch(`${config.public.apiBase}/api/orders/${orderId}/status`, {
      method: 'PATCH',
      body: JSON.stringify(statusValue),
      headers: {
        'Content-Type': 'application/json'
      }
    })
    await refresh()
  } catch (e) {
    const errorMessage = e instanceof Error ? e.message : 'Unknown error'
    console.error('Failed to update order status:', errorMessage)
    if (process.client) {
      alert('Failed to update order status: ' + errorMessage)
    }
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

.orders-list {
  display: grid;
  gap: 1.5rem;
}

.order-card {
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 1.5rem;
}

.order-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #f0f0f0;
}

h3 {
  color: #2c3e50;
  margin: 0;
}

.status {
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.9rem;
  font-weight: bold;
}

.status-pending {
  background: #fff3cd;
  color: #856404;
}

.status-processing {
  background: #d1ecf1;
  color: #0c5460;
}

.status-shipped {
  background: #d4edda;
  color: #155724;
}

.status-delivered {
  background: #d4edda;
  color: #155724;
}

.status-cancelled {
  background: #f8d7da;
  color: #721c24;
}

.order-info {
  margin-bottom: 1rem;
}

.order-info p {
  margin: 0.5rem 0;
  color: #34495e;
}

.order-items {
  margin-bottom: 1rem;
}

.order-items h4 {
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.order-item {
  display: flex;
  justify-content: space-between;
  padding: 0.5rem;
  background: #f8f9fa;
  margin-bottom: 0.25rem;
  border-radius: 4px;
}

.order-total {
  text-align: right;
  padding-top: 1rem;
  margin-bottom: 1rem;
  border-top: 2px solid #f0f0f0;
  font-size: 1.2rem;
}

.order-actions {
  display: flex;
  justify-content: flex-end;
}

.status-select {
  padding: 0.5rem 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
}

.status-select:focus {
  outline: none;
  border-color: #3498db;
}
</style>
