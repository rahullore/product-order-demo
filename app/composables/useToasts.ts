import { set } from "@nuxt/ui/runtime/utils/index.js";

export type ToastType = 'success' | 'error' | 'info';

export type Toast = {
    id: number
    message: string
    type: ToastType
    duration: number // in milliseconds
};

export const useToasts = () => {
    const toasts = useState<Toast[]>('toasts', () => [])

    function showToast(message: string, type: ToastType = 'info', duration: number = 3000) {
        const id = Date.now();
        toasts.value.push({ id, message, type, duration });

        setTimeout(() => {
            toasts.value = toasts.value.filter(toast => toast.id !== id);
        }, duration);
    }

    function removeToast(id: number) {
        toasts.value = toasts.value.filter(toast => toast.id !== id);
    }


    return {
        toasts,
        showToast,
        removeToast
    };
};