export type TopProduct = {
    name: string;
    totalQty: number,
    totalSpend: number
};

export type OrderInsights = {
    totalOrders: number;
    totalItems: number;
    totalSpend: number;
    topProducts: TopProduct[];
    lastUpdateUTC: string;
};

export const useInsights = () => {

    const {get} = useApi();

    const fetchInsights = async ()=>{
        return await get<OrderInsights>('/api/orders/insights');
    };

    return {
        fetchInsights
    };
}