import * as adapter from '../adapter.js'
import { ref, onMounted, toRaw, watch, computed } from 'vue';
import { dataStore } from '../dataStore.js'
import { useRouter } from 'vue-router';
import { showWarningToast, showErrorToast, showSuccessToast } from '../notifications.js'

export function pageSetup() {
    const store = dataStore();    
    const router = useRouter();

    const progress = ref(0);
    const maxProgress = ref(100);
    const processName = ref('');
    const stepName = ref('');
    const processViewerFile = ref('');
    const processViewerList = ref([]);
    const processEdit = ref({});
    const selectedProcesses = ref([])

    const fileTitleLabel = ref('');
    const openProcessViewerLabel = ref('');
    const processViewerTitleLabel = ref('');
    const saveLabel = ref('');
    const regenerateLabel = ref('');
    const processDetailLabel = ref('');
    const selectAllLabel = ref('');
    const unselectAllLabel = ref('');
    const refreshLabel = ref('');
    const runLabel = ref('');
    const processListLabel = ref('');
    const processHeaderLabel = ref('');
    const descriptionHeaderLabel = ref('');
    const folderHeaderLabel = ref('');
    const showInputHeaderLabel = ref('');
    const runHeaderLabel = ref('');
    const selectHeaderLabel = ref('');
    const editHeaderLabel = ref('');
    const deleteHeaderLabel = ref('');
    const copyLabel = ref('');
    const shortcutLabel = ref('');

    let viewerFileGlobal = null;
    let i18n;

    async function initApp() {
        i18n = {
            t: window.electronAPI.i18nT
        };

        await changeLanguage(store.getLanguage());

        viewerFileGlobal = await window.electronAPI.getGlobal('viewerFile');

        let baseDirectory = null;
        if (viewerFileGlobal)
        {
            processViewerFile.value = baseDirectory = viewerFileGlobal;
        }
        else
        {
            if (store.getProcessViewerFile() != null) {
                processViewerFile.value = baseDirectory = store.getProcessViewerFile();
                await adapter.setWorkSpaceDirectory(baseDirectory);
            }
            else {
                baseDirectory = await window.electronAPI.getBaseDirectory();
            }
        }
         
        await adapter.reloadList();
        processViewerList.value = await adapter.retrieveProcessViewerList();
        syncSelectedProcesses();
    }

    onMounted(async () => {
        await sleep(100);
        initApp();
    });

    watch(
        () => store.userSettings.Preferences.Language, async (newLang, oldLang) => {
            await sleep(100);
            changeLanguage(newLang);
        }
    );

    async function changeLanguage(lang) {
        await window.electronAPI.i18nInit('ProcessListView', lang);
        fileTitleLabel.value = await i18n.t('File');
        openProcessViewerLabel.value = await i18n.t('OpenProcessViewer');
        processViewerTitleLabel.value = await i18n.t('ProcessViewerTitle');
        saveLabel.value = await i18n.t('Save');
        regenerateLabel.value = await i18n.t('Regenerate');
        processDetailLabel.value = await i18n.t('ProcessDetail');
        selectAllLabel.value = await i18n.t('SelectAll');
        unselectAllLabel.value = await i18n.t('UnselectAll');
        refreshLabel.value = await i18n.t('Refresh');
        runLabel.value = await i18n.t('Run');
        processListLabel.value = await i18n.t('ProcessList');
        processHeaderLabel.value = await i18n.t('ProcessHeader');
        descriptionHeaderLabel.value = await i18n.t('DescriptionHeader');
        folderHeaderLabel.value = await i18n.t('FolderHeader');
        showInputHeaderLabel.value = await i18n.t('ShowInputHeader');
        runHeaderLabel.value = await i18n.t('RunHeader');
        editHeaderLabel.value = await i18n.t('editHeader');
        deleteHeaderLabel.value = await i18n.t('deleteHeader');
        selectHeaderLabel.value = await i18n.t('selectHeader');
        copyLabel.value = await i18n.t('copy');
        shortcutLabel.value = await i18n.t('shortcut');
    }

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    function goToProcessDetail(process) {
        router.push({
            path: '/ProcessDetail',
            query: {
                processName: process.ProcessName,
                processFile: process.ProcessFile,
                processFolder: process.Folder
            }
        })
    }
    
    const isAllSelected = computed({
        get() {
            if (processViewerList.value.length === 0) {
                return false;
            }

            return selectedProcesses.value.length === processViewerList.value.length;
        },
        set(value) {
            if (value) {
                selectedProcesses.value = processViewerList.value.map((process) => process.ProcessNumber);
            } else {
                selectedProcesses.value = []
            }
        }
    })

    const syncSelectedProcesses = () => {
        selectedProcesses.value = processViewerList.value
            .filter((process) => process.Run === true)
            .map((process) => process.ProcessNumber)
    }

    const saveClick = async () => {
        const resultConfirm = await window.electronAPI.showConfirm(
            "Are you sure you want to save?"
        );

        if (resultConfirm) {
            processViewerList.value.forEach((process) => {
                const processNumber = process.ProcessNumber
                process.Run = selectedProcesses.value.includes(processNumber);
            });

            const data = toRaw(processViewerList.value);

            if (data.length > 0) {
                await adapter.saveConfig(data);
                showSuccessToast("Process saved");
            }
        }
    }

    const openProcessViewerClick = async () => {
        const resultado = await window.electronAPI.openJsonFile();

        if (!resultado) {
            return;
        }

        viewerFileGlobal = processViewerFile.value = resultado.filePath;
        const baseDirectory = processViewerFile.value;

        await window.electronAPI.setGlobal('viewerFile', processViewerFile.value);
        await adapter.setWorkSpaceDirectory(baseDirectory);
        await adapter.reloadList();
        processViewerList.value = await adapter.retrieveProcessViewerList();

        store.setProcessViewerFile(processViewerFile.value);
        await adapter.saveUserSettings(toRaw(store.userSettings));
    }

    const runClick = async () => {

        if (!selectedProcesses.value.length) {
            await window.electronAPI.showAlert("Please select at least one process to run.");
            return;
        }

        const resultConfirm = await window.electronAPI.showConfirm(
            "Are you sure you want to run the processes?"
        );

        if (resultConfirm) {
            processViewerList.value.forEach((process) => {
                const processNumber = process.ProcessNumber
                process.Run = selectedProcesses.value.includes(processNumber);
            });

            const data = toRaw(processViewerList.value);

            if (data.length > 0) {
                await adapter.runProcessList(data);
                await sleep(1000);
                startPoll();
            }
        }
    }

    const copyProcessClick = async () => {
        if (!selectedProcesses.value.length) {
            await window.electronAPI.showAlert("Please select at least one process to copy.");
            return;
        }

        for (const processNumber of selectedProcesses.value) {
            await adapter.copyProcessByNumber({ ProcessNumber: processNumber });
        }

        await adapter.reloadList();
        processViewerList.value = await adapter.retrieveProcessViewerList();

        showSuccessToast("Process copied");
    }

    const createProcessShortcutClick = async () => {
        if (!selectedProcesses.value.length) {
            await window.electronAPI.showAlert("Please select at least one process to shortcut.");
            return;
        }

        const promises = selectedProcesses.value.map((processNumber) =>
            adapter.createProcessShortcutByNumber({ ProcessNumber: processNumber })
        )        
        await Promise.all(promises);

        showSuccessToast("Process shortcut created");
    }

    const saveProcessClick = async (process) => {
        const data = toRaw(process);
        const saved = await adapter.saveProcessItem(data);
        const oldprocessName = processEdit.value.ProcessName;        
        const item = processViewerList.value.find(x => x.ProcessNumber === process.ProcessNumber);

        if (saved) {
            item.ProcessFile = process.ProcessFile.substring(0, process.ProcessFile.lastIndexOf('/') + 1)
                            + process.ProcessName + '.json';
            
            showSuccessToast("Process saved");
        }
        else {
            item.ProcessName = oldprocessName;
        }
        
        processEdit.value = {};
    }

    const editProcessClick = async (process) => {
        const data = structuredClone(toRaw(process));
        processEdit.value = data;
    }

    const deleteProcessClick = async (processNumber) => {
        const resultConfirm = await window.electronAPI.showConfirm(
            "Are you sure you want to delete this process?"
        );

        if (resultConfirm) {
            await adapter.deleteProcessByNumber({ ProcessNumber: processNumber });
            await adapter.reloadList();
            processViewerList.value = await adapter.retrieveProcessViewerList();

            showWarningToast("Process deleted");
        }

        processEdit.value = {};
    }

    let shouldContinue = true;
    let polling = false;

    async function runningPoll() {
        if (!shouldContinue || polling) return;

        polling = true;

        try {
            const [
                progressValue,
                maxValue,
                logDescription,
                running
            ] = await Promise.all([
                adapter.getProgress(),
                adapter.getMaximumProgress(),
                adapter.getLog(),
                adapter.isRunning()
            ]);

            console.log("max:" + maxProgress.toString());
            console.log("progress:" + progress.toString());
            console.log("Log description:" + JSON.stringify(logDescription));


            if (maxValue != null) {
                maxProgress.value = maxValue;
            }

            if (progressValue != null) {
                progress.value = progressValue;
            }

            if (logDescription) {
                processName.value = logDescription.ProcessName + " - " + logDescription.TaskName;
                stepName.value = logDescription.StepName;
            }

            if (!running) {
                shouldContinue = false;
                return;
            }

        } catch (error) {
            console.error("Error in polling:", error);
            shouldContinue = false;
        } finally {
            polling = false;

            if (shouldContinue) {
                setTimeout(runningPoll, 500);
            }
        }
    }

    async function startPoll() {
        shouldContinue = true;
        polling = false
        runningPoll();
    }

    return {
        isAllSelected,
        selectedProcesses,
        progress,
        maxProgress,
        processName,
        stepName,
        processViewerFile,
        processViewerList,
        processEdit,
        fileTitleLabel,
        openProcessViewerLabel,
        processViewerTitleLabel,
        saveLabel,
        regenerateLabel,
        processDetailLabel,
        copyLabel,
        shortcutLabel,
        runLabel,
        processListLabel,
        processHeaderLabel,
        folderHeaderLabel,
        showInputHeaderLabel,
        editHeaderLabel,
        deleteHeaderLabel,
        goToProcessDetail,
        saveClick,
        runClick,
        openProcessViewerClick,
        saveProcessClick,
        editProcessClick,
        deleteProcessClick,
        createProcessShortcutClick,
        copyProcessClick,
    };
}