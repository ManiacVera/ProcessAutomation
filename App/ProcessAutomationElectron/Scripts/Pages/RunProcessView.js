import * as adapter from '../adapter.js'
import { ref, onMounted, toRaw, watch } from 'vue';
import { useRouter } from 'vue-router';
import { dataStore } from '../dataStore.js'
import { storeToRefs } from 'pinia'
import { showWarningToast, showErrorToast, showSuccessToast } from '../notifications.js'

export function pageSetup() {
    const store = dataStore();
    const { data: model } = storeToRefs(store);
    const router = useRouter();

    const runProcessTitle = ref('');
    const btnRunProcessLabel = ref('');
    const btnAbortProcessLabel = ref('');
    const btnBackLabel = ref('');
    const processNameLabel = ref('');
    const taskHeader = ref('');
    const descriptionHeader = ref('');
    const valueHeader = ref('');
    const valueFormatHeader = ref('');
    const runProcessListLabel = ref('');

    let stepValueRef = ref(null);
    let i18n;
    let runProcessList = ref([]);

    let maxProgress = ref('');
    let progress = ref('');
    let progressText = ref('');

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

    async function initApp() {
        i18n = {
            t: window.electronAPI.i18nT
        };

        await changeLanguage(store.getLanguage());

        if (store.data && store.data.ProcessData) {
            loadPage();
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
        await window.electronAPI.i18nInit('RunProcessView', lang);
        runProcessTitle.value = await i18n.t('RunProcessInformation');
        btnRunProcessLabel.value = await i18n.t('Run');
        btnAbortProcessLabel.value = await i18n.t('Abort');
        btnBackLabel.value = await i18n.t('Back');
        processNameLabel.value = await i18n.t('ProcessName');
        taskHeader.value = await i18n.t('TaskHeader');
        descriptionHeader.value = await i18n.t('DescriptionHeader');
        valueHeader.value = await i18n.t('ValueHeader');
        valueFormatHeader.value = await i18n.t('ValueFormatHeader');
        runProcessListLabel.value = await i18n.t('RunProcessListInformation');
    }

    async function loadPage() {
        runProcessList.value = await adapter.initProcess(toRaw(store.data));
    }

    async function stepFormatValueClick(runProcessItem) {
        stepValueRef.value = runProcessItem;
        // stepOutputResultRef.value = null;
        // componentValueRef.value = null;

        // const stepList = task.value.StepList.map(item => ({
        //     Number: item.Number,
        //     Variable: item.Variable
        // }));

        // const taskInfo = {
        //     StepList: stepList
        // }

        // const variableList = utils.retrieveStepVariableList(runProcessItem.StepNumberLegacy, taskInfo, toRaw(store.data.ProcessData));
        const componentInfo = toRaw(runProcessItem.ComponentInfo);

        const buildInputFormatData = {
            Page: "InputPage.html",
            ItemCode: String(runProcessItem.StepNumber),
            Value: toBase64(runProcessItem.InputValue),
            ValueAction: "STEP",
            Component: componentInfo.Name,
            Description: runProcessItem.StepDescription,
            VariableList: [],
            Language: store.getLanguage(),
            FormatFile: "InputFields.html",
            RootComponentFolder: "",
            EnabledOptions: false,
            ComponentData: componentInfo,
        };

        window.electronAPI.openPopup("Pages/InputPage.html", buildInputFormatData);
    }

    async function updateInputValue() {
        runProcessList.value.forEach(async item => {
                await adapter.setInputValue({
                    StepNumber: item.StepNumber,
                    InputValue: item.InputValue
                }
            );
        });
    }

    window.electronAPI.onPopupResult(async (result) => {
        console.log("Resultado del popup:", result);

        if (result && result.Result) {
            const isTextField = result.Result.hasOwnProperty('isTextField');
            const resultValue = isTextField ? result.Result.value : result.Result;

            if (stepValueRef.value) {
                stepValueRef.value.InputValue = resultValue;
            }
        }
    });

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    async function runProcessClick() {
        await updateInputValue();

        const running = await adapter.isRunning();

        if (!running) {
            const resultConfirm = await window.electronAPI.showConfirm(
                "Are you sure you want to run the process?"
            );

            if (resultConfirm) {
                progress.value = 0;
                progressText.textContent = "";

                await adapter.runProcess();
                await sleep(1000);
                startPoll();
            }
        }
    }

    async function abortProcessClick() {
        const running = await adapter.isRunning();

        if (running) {
            const confirmAbort = await window.electronAPI.showConfirm(
                "Are you sure you want to abort the process?"
            );

            if (confirmAbort) {
                adapter.abortProcess();
                showErrorToast("Process aborted");
            }
        }
    }

    async function backClick() {
        goToProcessPage();
    }

    function goToProcessPage() {
        router.push({
            path: '/Process'
        })
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
                adapter.getStepDescription(),
                adapter.isRunning()
            ]);

            console.log("max:" + maxValue.toString());
            console.log("progress:" + progressValue.toString());
            console.log("Log description:" + JSON.stringify(logDescription));

            if (maxValue != null) {
                maxProgress.value = maxValue;
            }

            if (progressValue != null) {
                progress.value = progressValue;
            }

            if (logDescription) {
                progressText.value = logDescription.Description;
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
        model,
        progress,
        maxProgress,
        progressText,
        runProcessList,
        runProcessTitle,
        btnRunProcessLabel,
        btnAbortProcessLabel,
        btnBackLabel,
        processNameLabel,
        taskHeader,
        descriptionHeader,
        valueHeader,
        valueFormatHeader,
        runProcessListLabel,
        stepFormatValueClick,
        runProcessClick,
        abortProcessClick,
        backClick
    }

}