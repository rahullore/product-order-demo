type HttpMethod = 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH';

export const useApi=()=>{
    const config = useRuntimeConfig();
    const apiBase = config.public.apiBase;

    return{
        apiBase,
        async get<T>(path: string){
            return await $fetch<T>(`${apiBase}${path}`);
        },

        async request<T>(path:string, options: {method: HttpMethod, body?: any}){
            return await $fetch<T>(`${apiBase}${path}`, {
                method: options.method,
                body: options.body,
            });
        },

        async post<T>(path: string, body?: any){
            return await $fetch<T>(`${apiBase}${path}`, {
                method: 'POST',
                body,
            })
        },

        async del<T>(path: string){
            return await $fetch<T>(`${apiBase}${path}`, {
                method: 'DELETE',
            })
        }
    }
}