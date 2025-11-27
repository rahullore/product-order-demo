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
  const orders = useState<Order[]>(STORAGE_KEY, () => {
    if (process.client) {
        const storedOrders = localStorage.getItem(STORAGE_KEY);
        return storedOrders ? JSON.parse(storedOrders) : [];
        }
        return [];
  });

  const syncOrderFromStorage = () => {
    if (process.client) {
      const storedOrders = localStorage.getItem(STORAGE_KEY);
      if (storedOrders) {
      orders.value = storedOrders ? JSON.parse(storedOrders) : [];
      }
    }
  };

  if(!process.client){
    window.addEventListener('storage', (event) => {
        if (event.key === STORAGE_KEY) {
            console.log('Storage event detected for orders. Syncing...');
            syncOrderFromStorage();
        }
    });
    syncOrderFromStorage();
  }

  function saveOrdersToStorage(currentOrders: Order[]) {
    if (process.client) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(currentOrders));
    }
  }

  function addOrder(order: Omit<Order, 'id' | 'createdAt'>) {
    const newOrder: Order = {
      id: Date.now(),
      createdAt: new Date().toISOString(),
      ...order
    };

    // Replace the array to keep reactivity predictable.
    orders.value = [...orders.value, newOrder];
    //orders.value.push(newOrder);
    console.log('Order added:', orders.value);
    saveOrdersToStorage(orders.value);
  }

  return {
    orders,
    addOrder
  };
};
