import { ref, onMounted } from 'vue';
import { dataStore } from '../dataStore.js'
import * as adapter from '../adapter.js'

export function pageSetup() { 
    const store = dataStore();
    const titleLabel = ref('Process Viewer');
    const buttons = ref([]);
    const message = ref('');
    let i18n;

    async function initApp() {
        i18n = {
            t: window.electronAPI.i18nT
        };

        window.electronAPI.onDialogInit(async (data) => {
            store.setUserSettings(await adapter.loadUserSettings());
            await changeLanguage(store.getLanguage());
            titleLabel.value = data.title;
            message.value = data.message;
            
            buttons.value = await Promise.all(
                data.buttons.map(async (btn) => {
                    try {
                        return await i18n.t(btn) || btn;
                    } catch {
                        return btn;
                    }
                })
            );
        });
    }

    onMounted(async () => {
        initApp();
    });

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    async function changeLanguage(lang) {
        await window.electronAPI.i18nInit('DialogPage', lang);
    }

    const selectButton = (index) => {
        window.electronAPI.sendDialogResult(index);
    };

    return {
        titleLabel,
        buttons,
        message,
        selectButton
    };
}