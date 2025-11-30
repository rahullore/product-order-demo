export type Order = {
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

  return {
    orders,
    addOrder
  };
};
