import * as adapter from '../adapter.js';
import { ref, onMounted, toRaw, watch } from 'vue';
import { dataStore } from '../dataStore.js'
import { useRoute, useRouter } from 'vue-router';

export function pageSetup() {
    const store = dataStore();
    const route = useRoute();
    const router = useRouter();

    const processNameLabel = ref('');
    const processTitleLabel = ref('');
    const acceptLabel = ref('');
    const cancelLabel = ref('');
    const processListLabel = ref('');
    const taskNameHeaderLabel = ref('');
    const stepDescriptionHeaderLabel = ref('');
    const inputValueFormatHeaderLabel = ref('');
    const valueFormatHeaderLabel = ref('');
    const outputValueTypeHeaderLabel = ref('');

    const processName = route.query.processName;
    const processFile = route.query.processFile;
    const processFolder = route.query.processFolder;

    const processList = ref([]);
    const selectedProcess = ref(null);
    let i18n;

    async function initApp() {
        i18n = {
            t: window.electronAPI.i18nT
        };

        await changeLanguage(store.getLanguage());
        await loadInputGrid();
    }

    onMounted(async () => {
        initApp();
    });

    watch(
        () => store.userSettings.Preferences.Language, async (newLang, oldLang) => {
            await sleep(100);
            changeLanguage(newLang);
        }
    );

    async function changeLanguage(lang) {
        await window.electronAPI.i18nInit('ProcessDetailView', lang);
        processNameLabel.value = await i18n.t('ProcessName');
        processTitleLabel.value = await i18n.t('ProcessTitleLabel');
        acceptLabel.value = await i18n.t('Accept');
        cancelLabel.value = await i18n.t('Cancel');
        processListLabel.value = await i18n.t('ProcessList');
        taskNameHeaderLabel.value = await i18n.t('TaskNameHeader');
        stepDescriptionHeaderLabel.value = await i18n.t('StepDescriptionHeader');
        inputValueFormatHeaderLabel.value = await i18n.t('InputValueFormatHeader');
        valueFormatHeaderLabel.value = await i18n.t('ValueFormatHeader');
        outputValueTypeHeaderLabel.value = await i18n.t('OutputValueTypeHeader');
    }

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    function toBase64(str) {
        const bytes = new TextEncoder().encode(str);
        let binary = "";

        bytes.forEach(b => binary += String.fromCharCode(b));

        return btoa(binary);
    }

    function fromBase64(base64) {
        const binary = atob(base64);

        const bytes = Uint8Array.from(
            binary,
            char => char.charCodeAt(0)
        );

        return new TextDecoder().decode(bytes);
    }

    const acceptClick = async () => {
        if (processList.value && processList.value.length > 0) {
            const resultConfirm = await window.electronAPI.showConfirm(
                "Are you sure you want to save this process?"
            );

            if (resultConfirm) {
                const data = processList.value.map(item => ({
                    TaskLegacyNumber: item.TaskLegacyNumber,
                    StepLegacyNumber: item.StepLegacyNumber,
                    InputValue: item.InputValue
                }));

                if (data.length > 0) {
                    await adapter.saveProcessConfig(data);
                    goBackClick();
                }
            }
        }
        else {
            goBackClick();
        }
    }

    const goBackClick = () => {
        router.push({
            path: '/ProcessList'
        })
    }

    async function loadInputGrid() {
        processList.value = await adapter.readProcessFile(processFile);
    }

    async function showInputFormat(process) {
        selectedProcess.value = process;
        const variableList = [];

        const buildInputFormatData = {
            Page: "InputPage.html",
            ItemCode: String(process.StepNumber),
            Value: toBase64(process.InputValue),
            ValueAction: "STEP",
            Component: process.ComponentInfo.Name,
            Description: process.StepDescription,
            VariableList: variableList,
            Language: store.getLanguage(),
            FormatFile: "InputFields.html",
            RootComponentFolder: processFolder,
            EnabledOptions: false,
            ComponentData: toRaw(process.ComponentInfo),
        };

        window.electronAPI.openPopup("Pages/InputPage.html", buildInputFormatData);
    }

    window.electronAPI.onPopupResult((result) => {

        if (result && result.ItemCode) {
            const isTextField = result.Result.hasOwnProperty('isTextField');
            const resultValue = isTextField ? result.Result.value : result.Result; 

            if (result.Page == "InputPage.html") {
                if (result.ValueAction == "STEP") {
                    selectedProcess.value.InputValue = resultValue;
                }
            }
        }

        selectedProcess.value = null;
    });

    return {
        processName,
        processNameLabel,
        processTitleLabel,
        acceptLabel,
        cancelLabel,
        processListLabel,
        taskNameHeaderLabel,
        stepDescriptionHeaderLabel,
        inputValueFormatHeaderLabel,
        valueFormatHeaderLabel,
        outputValueTypeHeaderLabel,
        processList,
        acceptClick,
        goBackClick,
        showInputFormat
    };
}