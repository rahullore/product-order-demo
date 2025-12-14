/*export type Order = {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  price: number;
  createdAt: string;
};

const STORAGE_KEY = 'my-orders';

export const useOrders = () => {
  
  const orders = useState<Order[]>('orders', () => {
    if (process.client) {
      const storedOrders = localStorage.getItem(STORAGE_KEY);
      return storedOrders ? JSON.parse(storedOrders) : [];
    }
    return [];
  });

  const { showToast } = useToasts();

  function persist() {
    if (process.client) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(orders.value));
    }
  }

  function addOrder(order: Omit<Order, 'id' | 'createdAt'>) {
    const newOrder: Order = {
      id: Date.now(),
      createdAt: new Date().toISOString(),
      ...order
    };
    orders.value.push(newOrder);
    persist();
    console.log('Order added:', newOrder);
    showToast(`Order for ${newOrder.productName} added successfully!`, 'success');
  }

  function clearOrders(){
    if(orders.value.length === 0) return;
    orders.value = [];
    persist();
    showToast('All orders cleared.', 'info');
  }

  return {
    orders,
    addOrder,
    clearOrders
  };
};*/

import { da } from "@nuxt/ui/runtime/locale/index.js";

export type Order = {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  price: number;
  createdAt: string;
};
type CreateOrderRequest ={
  productId: number;
  quantity: number;
}

export const useOrders = () => {
  const { post,get, del } = useApi();
  const { showToast } = useToasts();

  const orders = useState<Order[]>('orders',() => []);

  const loading = ref(false);
  const error = ref<string | null>(null);

  async function loadOrders() {
    loading.value = true;
    error.value = null;

    try{
      const data = await get<Order[]>('/api/orders');
      orders.value = data;
    }
    catch(e:any){
      error.value = e?.message??'failed to load orders';
      showToast('Failed to load orders','error');
    }finally{
      loading.value = false;
    }
  }


  async function addOrder(request: CreateOrderRequest) {
    loading.value = true;
    error.value = null;
    
    try{
      console.log('Adding order with request:', request.productId, request.quantity);
      const newOrder = await post<Order>('/api/orders',request);
      orders.value.unshift(newOrder);
      showToast(`Order for ${newOrder.productName} added successfully!`, 'success');
    }
    catch(e:any){
      error.value = e?.message??'failed to add order';
      showToast('Failed to add order','error');
    }finally{
      loading.value = false;
    } 
  }

  async function clearOrders(){
   if(orders.value.length === 0) return;
   loading.value = true;
   error.value = null;
   
   try{
      await del<void>('/api/orders');
      orders.value = [];
      showToast('All orders cleared.', 'info');
    }
    catch(e:any){
      error.value = e?.message??'failed to clear orders';
      showToast('Failed to clear orders','error');
    }finally{
      loading.value = false;
    }
  }

  return {
    orders,
    loading,
    error,
    loadOrders,
    addOrder,
    clearOrders
  };
}