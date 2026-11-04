import * as adapter from '../adapter.js'
import { ref, onMounted, toRaw, watch } from 'vue';
import { useRouter } from 'vue-router';
import { dataStore } from '../dataStore.js'
import { storeToRefs } from 'pinia'
import { showWarningToast, showErrorToast, showSuccessToast } from '../notifications.js'

export function pageSetup() {
    const globalStore = {
        set: (k, v) => window.electronAPI.setGlobal(k, v),
        get: (k) => window.electronAPI.getGlobal(k)
    };

    const store = dataStore();
    const { data: model } = storeToRefs(store);

    const router = useRouter();
    const processTitle = ref('');
    const lblNumber = ref('');
    const lblProcessName = ref('');
    const btnOpen = ref('');
    const btnClose = ref('');
    const btnClean = ref('');
    const btnOpenDataFolder = ref('');
    const btnSave = ref('');
    const btnBuild = ref('');
    const btnRun = ref('');
    const btnImport = ref('');
    const taskTitle = ref('');
    const btnAddTaskTop = ref('');
    const numberHeader = ref('');
    const nameHeader = ref('');
    const enabledHeader = ref('');
    const editHeader = ref('');
    const deleteHeader = ref('');
    const btnAddTaskBottom = ref('');
    const editLabel = ref('');
    const deleteLabel = ref('');
    const processList = ref([]);
    const isProcessFileDisabled = ref(false);

    let i18n;

    async function initApp() {
        i18n = {
            t: window.electronAPI.i18nT
        };

        await changeLanguage(store.getLanguage());
        await adapter.setDefaultSettings({ "PipeName": window.electronAPI.pipeName });
        processList.value = await adapter.retrieveProcessList(" ", " ");

        if (await globalStore.get("processDisabled")) {
            isProcessFileDisabled.value = await globalStore.get("processDisabled");
        }
    }

    onMounted(async () => {
        await store.loadLocalData();
        initApp();
    });

    watch(
        () => store.userSettings.Preferences.Language, async (newLang, oldLang) => {
            await sleep(100);
            changeLanguage(newLang);
        }
    );

    async function changeLanguage(lang) {        
        await window.electronAPI.i18nInit('ProcessView', lang);
        processTitle.value = await i18n.t('ProcessInformation');
        btnAddTaskTop.value = await i18n.t('AddTask');
        btnAddTaskBottom.value = await i18n.t('AddTask');
        btnOpen.value = await i18n.t('Open');
        btnClose.value = await i18n.t('Close');
        btnClean.value = await i18n.t('Clean');
        btnOpenDataFolder.value = await i18n.t('OpenDataFolder');
        btnBuild.value = await i18n.t('Build');
        btnRun.value = await i18n.t('Run');
        lblNumber.value = await i18n.t('Number');
        lblProcessName.value = await i18n.t('ProcessName');
        btnImport.value = await i18n.t('Import');
        btnSave.value = await i18n.t('Save');
        numberHeader.value = await i18n.t('NumberHeader');
        nameHeader.value = await i18n.t('NameHeader');
        enabledHeader.value = await i18n.t('EnabledHeader');
        editHeader.value = await i18n.t('EditHeader');
        deleteHeader.value = await i18n.t('DeleteHeader');
        taskTitle.value = await i18n.t('TaskList');
        editLabel.value = await i18n.t('Edit');
        deleteLabel.value = await i18n.t('Delete');
    }

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    async function newProcess() {
        store.initData();
        isProcessFileDisabled.value = false;
        await globalStore.set("processDisabled", isProcessFileDisabled.value);
    }

    async function editTaskClick(taskNumber) {
        goToTaskPage(taskNumber);
    }

    async function deleteTaskClick(taskNumber) {
        const confirmDelete = await window.electronAPI.showConfirm(
            "Are you sure you want to delete this task?"
        );

        if (!confirmDelete) return;

        try {
            const list = store.data.ProcessData.TaskList;
            const index = list.findIndex(t => t.Number === Number(taskNumber));

            if (index !== -1) {
                list.splice(index, 1);
                await store.saveLocalData();
            }
        } catch (err) {
            console.error("Error deleting task:", err);
        }
    }

    async function openClick() {
        if (store.data.ProcessFile) {            
            const processInfo = processList.value.find(t => t.Value === store.data.ProcessFile);
            if (processInfo && processInfo.Value) {
                store.setProcessFile(processInfo.Text);
                let data = await adapter.openProcess(processInfo.Value, " ", " ");

                if (data) {
                    store.setData(data);
                    await store.saveLocalData();
                    isProcessFileDisabled.value = true;
                    await globalStore.set("processDisabled", isProcessFileDisabled.value);
                }
            }
        }
    }

    async function closeClick() {
        const resultConfirm = await window.electronAPI.showConfirm(
            "Are you sure you want to close process?"
        );

        if (resultConfirm) {
            await newProcess();
        }
    }

    async function importClick() {
        const result = await window.electronAPI.openProcessFile();

        if (!result) {
            return;
        }

        store.setProcessFile(result.fileName);
        let data = await adapter.openProcess(result.filePath, " ", " ");

        if (data) {
            store.setData(data);
            await store.saveLocalData();
            isProcessFileDisabled.value = true;
            await globalStore.set("processDisabled", isProcessFileDisabled.value);

            showSuccessToast("Process imported");
        }
    }

    async function saveClick() {
        const resultConfirm = await window.electronAPI.showConfirm(
            "Are you sure you want to save this process?"
        );

        if (resultConfirm) {
            if (model) {
                if (!store.data.ProcessFile)
                    store.setProcessFile(" ");

                await store.saveLocalData();
                const result = await adapter.saveProcess(toRaw(store.data));
                if (result) {
                    if (!result.toLowerCase().startsWith("error")) {
                        store.setProcessFile(result);
                        await store.saveLocalData();
                        processList.value = await adapter.retrieveProcessList(" ", " ");
                        isProcessFileDisabled.value = true;
                        await globalStore.set("processDisabled", isProcessFileDisabled.value);
                        
                        showSuccessToast("Process saved");
                    }
                    else {
                        showErrorToast(result);
                    }
                }
            }
        }
    }

    async function cleanClick() {
        const resultConfirm = await window.electronAPI.showConfirm(
            "Are you sure you want to clean the entire form for a new process?"
        );

        if (resultConfirm) {
            await newProcess();
        }
    }

    async function openDataFolderClick() {
        const dataPath = await adapter.getDataPath();
        await window.electronAPI.openPath(dataPath);
    }

    async function buildClick() {
        const resultConfirm = await window.electronAPI.showConfirm(
            "Are you sure you want to build process?"
        );

        if (resultConfirm) {
            if (model) {
                await store.saveLocalData();
                const resultSaved = await adapter.saveProcess(toRaw(store.data));
                
                if (resultSaved && !resultSaved.toLowerCase().startsWith("error")) {
                    let copyData = JSON.parse(JSON.stringify(store.data));
                    copyData.UserCode = "Default";

                    const result = await adapter.buildProcess(copyData);
                    if (result) {
                        if (!result.toLowerCase().startsWith("error")) {
                            await window.electronAPI.showAlert("Builded process");

                            const dataPath = await adapter.getDataPath();
                            await window.electronAPI.openPath(dataPath);
                        }
                        else {
                            await window.electronAPI.showAlert(result);
                        }
                    }
                }
                else {
                    await window.electronAPI.showAlert(result);
                }
            }
        }
    }

    async function runClick() {
        await store.saveLocalData();
        goToRunProcessPage();
    }

    async function addTaskClick() {
        await store.saveLocalData();
        goToTaskPage(0);
    }

    function goToRunProcessPage() {
        router.push({
            path: '/RunProcess'
        });
    }

    function goToTaskPage(taskNumber) {
        router.push({
            path: '/Task',
            query: {
                taskNumber: taskNumber
            }
        });
    }

    return {
        model,
        processTitle,
        lblNumber,
        lblProcessName,
        btnOpen,
        btnClose,
        btnClean,
        btnOpenDataFolder,
        btnSave,
        btnBuild,
        btnRun,
        btnImport,
        taskTitle,
        btnAddTaskTop,
        numberHeader,
        nameHeader,
        enabledHeader,
        editHeader,
        deleteHeader,
        btnAddTaskBottom,
        editLabel,
        deleteLabel,
        processList,
        isProcessFileDisabled,
        editTaskClick,
        deleteTaskClick,
        openClick,
        closeClick,
        importClick,
        saveClick,
        cleanClick,
        openDataFolderClick,
        buildClick,
        runClick,
        addTaskClick
    };
}