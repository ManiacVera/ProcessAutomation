import * as adapter from '../adapter.js'
import { ref, onMounted, toRaw } from 'vue';
import { dataStore } from '../dataStore.js'
import { storeToRefs } from 'pinia'

export function pageSetup() {    
    const store = dataStore();
    const { userSettings } = storeToRefs(store);
    
    const languageLabel = ref('');
    const languageList = ref([]);
    let i18n;

    async function initApp() {
        store.setUserSettings(await adapter.loadUserSettings());

        i18n = {
            t: window.electronAPI.i18nT
        };

        await changeLanguage(store.getLanguage());
        languageList.value = await adapter.retrieveLanguageList();
    }

    onMounted(async () => {
        initApp();
    });

    async function changeLanguage(lang) {
        if (store.userSettings)
        {
            await adapter.saveUserSettings(toRaw(store.userSettings));
            await window.electronAPI.i18nInit('MasterView', lang);
            languageLabel.value = await i18n.t('Language');
        }
    }

    async function onChangeLanguage() {
        changeLanguage(store.userSettings.Preferences.Language);
    }

    return { 
        userSettings,
        languageLabel,
        languageList,
        onChangeLanguage
    };
}