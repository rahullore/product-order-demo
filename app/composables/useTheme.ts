export type Theme = 'light' | 'dark';

export const useTheme = () => {
    const theme = useState<Theme>('theme', () => 'light');

    function applyTheme(newTheme: Theme) {
        if(!process.client) return

        const root = document.documentElement;
        root.classList.toggle('dark', newTheme === 'dark');
        localStorage.setItem('theme', newTheme)
    }

    if (process.client) {
        const stored = localStorage.getItem('theme') as Theme | null;
        if(stored && stored !== theme.value) {
            theme.value = stored;
        }
        applyTheme(theme.value);
    }

    function toggleTheme() {
        theme.value = theme.value === 'light' ? 'dark' : 'light';
        applyTheme(theme.value);
    }

    return {
        theme,
        toggleTheme
    };
}