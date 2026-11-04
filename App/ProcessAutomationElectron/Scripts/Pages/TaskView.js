import * as adapter from '../adapter.js'
import * as utils from '../utilsProcess.js'
import { ref, onMounted, toRaw, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { dataStore } from '../dataStore.js'

export function pageSetup() {
    const store = dataStore();
    const route = useRoute();
    const router = useRouter();

    const task = ref({
        StepList: [],
        ComponentList: []
    });

    const taskTitle = ref('');
    const btnAddStepTop = ref('');
    const btnAddStepBottom = ref('');
    const lblTask = ref('');
    const lblDelay = ref('');
    const lblNumber = ref('');
    const lblEnabled = ref('');
    const btnAccept = ref('');
    const btnCancel = ref('');
    const tabStep = ref('');
    const tabComponent = ref('');
    const tabDeclaration = ref('');
    const btnAddComponentTop = ref('');
    const btnAddComponentBottom = ref('');
    const componentHeader = ref('');
    const descriptionHeader = ref('');
    const encapsulatedHeader = ref('');
    const variableHeader = ref('');
    const publicHeader = ref('');
    const valueHeader = ref('');
    const valueFormatHeader = ref('');
    const applyFormulaHeader = ref('');
    const valueInputTypeHeader = ref('');
    const valueOutputTypeHeader = ref('');
    const outputValueOptionHeader = ref('');
    const declarationTypeHeader = ref('');
    const delayHeader = ref('');
    const requiredHeader = ref('');
    const asyncHeader = ref('');
    const logHeader = ref('');
    const disableHeader = ref('');
    const deleteHeader = ref('');
    const componentTypeHeaderComponent = ref('');
    const componentHeaderComponent = ref('');
    const descriptionHeaderComponent = ref('');
    const valueHeaderComponent = ref('');
    const valueFormatComponent = ref('');
    const valueInputTypeHeaderComponent = ref('');
    const deleteHeaderComponent = ref('');
    const editLabel = ref('');
    const deleteLabel = ref('');

    let taskNumber = 0;
    let originalTask = null;
    let componentList = ref([]);
    let declarationTypeList = ref([]);
    let valueTypeList = ref([]);
    let outputValueList = ref([]);
    let encapsulatedPartitionList = ref([]);
    let componentTypeList = ref([]);
    let i18n;
    let draggedIndex = null;
    let stepValueRef = ref(null);
    let stepOutputResultRef = ref(null);
    let componentValueRef = ref(null);

    function getNextId(list, idField) {
        const ids = list
            .map(obj => obj[idField])
            .filter(id => typeof id === "number")
            .sort((a, b) => a - b);

        let newId = 1;

        for (let id of ids) {
            if (id === newId) {
                newId++;
            } else if (id > newId) {
                break;
            }
        }

        return newId;
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

    async function initApp() {
        i18n = {
            t: window.electronAPI.i18nT
        };

        await changeLanguage(store.getLanguage());
        await loadPage();
        generateComponentTypeList();
        syncStepComponentNamesByText();
    }

    onMounted(async () => {
        await store.loadLocalData();

        taskNumber = Number(route.query.taskNumber);
        if (taskNumber > 0) {
            task.value = store.data.ProcessData.TaskList.find(
                t => t.Number === taskNumber
            );

             originalTask = JSON.parse(JSON.stringify(task.value));
        } else {
            const newTask = {
                Number: getNextId(store.data.ProcessData.TaskList, "Number"),
                Name: '',
                Delay: 0,
                Enabled: true,
                StepList: [],
                ComponentList: []
            };

            store.data.ProcessData.TaskList.push(newTask);

            taskNumber = newTask.Number;
            task.value = newTask;
        }

        task.value.ComponentList.forEach((component, index) => {
            component._uid = Number(Date.now()) + index;
        });

        task.value.StepList.forEach((step) => {
            step._componentSelected = '';
        });

        initApp();
    });

    watch(
        () => store.userSettings.Preferences.Language, async (newLang, oldLang) => {
            await sleep(100);
            changeLanguage(newLang);
        }
    );

    async function changeLanguage(lang) {
        await window.electronAPI.i18nInit('TaskView', lang);
        taskTitle.value = await i18n.t('TaskInformation');
        btnAddStepTop.value = await i18n.t('AddStep');
        btnAddStepBottom.value = await i18n.t('AddStep');
        lblTask.value = await i18n.t('TaskName');
        lblDelay.value = await i18n.t('Delay');
        lblNumber.value = await i18n.t('Number');
        lblEnabled.value = await i18n.t('Enabled');
        btnAccept.value = await i18n.t('Accept');
        btnCancel.value = await i18n.t('Cancel');
        tabStep.value = await i18n.t('StepList');
        tabComponent.value = await i18n.t('ComponentList');
        tabDeclaration.value = await i18n.t('SentenceStatement');
        btnAddComponentTop.value = await i18n.t('AddComponent');
        btnAddComponentBottom.value = await i18n.t('AddComponent');
        componentHeader.value = await i18n.t('ComponentHeader');
        descriptionHeader.value = await i18n.t('DescriptionHeader');
        encapsulatedHeader.value = await i18n.t('EncapsulatedTypeHeader');
        variableHeader.value = await i18n.t('VariableHeader');
        publicHeader.value = await i18n.t('IsPublicVariableHeader');
        valueHeader.value = await i18n.t('ValueHeader');
        valueFormatHeader.value = await i18n.t('ValueFormatHeader');
        applyFormulaHeader.value = await i18n.t('ApplyFormulaHeader');
        valueInputTypeHeader.value = await i18n.t('ValueTypeHeader');
        valueOutputTypeHeader.value = await i18n.t('ValueOutputTypeHeader');
        outputValueOptionHeader.value = await i18n.t('OutputValueOptionHeader');
        declarationTypeHeader.value = await i18n.t('DeclarationTypeHeader');
        delayHeader.value = await i18n.t('DelayHeader');
        requiredHeader.value = await i18n.t('RequiredHeader');
        asyncHeader.value = await i18n.t('AsyncHeader');
        logHeader.value = await i18n.t('LogHeader');
        disableHeader.value = await i18n.t('DisableHeader');
        deleteHeader.value = await i18n.t('DeleteHeader');

        componentTypeHeaderComponent.value = await i18n.t('TypeHeader');
        componentHeaderComponent.value = await i18n.t('ComponentHeader');
        descriptionHeaderComponent.value = await i18n.t('DescriptionHeader');
        valueHeaderComponent.value = await i18n.t('ValueHeader');
        valueFormatComponent.value = await i18n.t('ValueFormatHeader');
        valueInputTypeHeaderComponent.value = await i18n.t('ValueTypeHeader');
        deleteHeaderComponent.value = await i18n.t('DeleteHeader');
        editLabel.value = await i18n.t('Edit');
        deleteLabel.value = await i18n.t('Delete');
    }

    async function loadPage() {
        await loadData();
    }

    function sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    function onComponentChange(event, step) {
        const selectedOption = componentTypeList.value.find(opt => opt.Value === step._componentSelected);
        step.Component = selectedOption ? selectedOption.Text : '';
    }

    function syncStepComponentNamesByUID() {
        const uidMap = new Map(componentTypeList.value.map(opt => [opt.Value, opt.Text]));

        task.value.StepList.forEach(step => {
            const uid = step._componentSelected;
            if (uid !== null && uidMap.has(uid)) {
                step.Component = uidMap.get(uid);
            } else {
                step.Component = '';
            }
        });
    }

    function syncStepComponentNamesByText() {
        const textToUidMap = new Map(componentTypeList.value.map(opt => [opt.Text, opt.Value]));

        task.value.StepList.forEach(step => {
            if (step.Component && textToUidMap.has(step.Component)) {
                step._componentSelected = textToUidMap.get(step.Component);
            }
        });
    }

    function updateNumber() {
        task.value.StepList
            .forEach((item, index) => {
                item.Number = index + 1;
        });
    }

    async function loadData() {
        componentList.value = await adapter.retrieveComponent();

        declarationTypeList.value = await adapter.retrieveDeclarationType();
        declarationTypeList.value.unshift({ Value: "", Text: "" });

        valueTypeList.value = await adapter.retrieveValueType();
        outputValueList.value = await adapter.retrieveValueOutputType();
        encapsulatedPartitionList.value = await adapter.retrieveEncapsulatedPartitionType();

        generateComponentTypeList();
    }

    function createStep() {
        return {
            EncapsulatedId: 0,
            EncapsulatedPartitionType: '',
            Number: 0,
            Description: '',
            Variable: '',
            IsPublicVariable: false,
            Component: '',
            Value: '',
            InputValueType: '',
            ValueAction: '',
            ApplyFormula: false,
            OutputValueType: '',
            OutputResultOption: '',
            ValueTypeDeclaration: '',
            Delay: 0,
            Required: true,
            Async: false,
            Log: false,
            Disable: false,
            ValueOptionList: [],
            ParameterList: [],
            _componentSelected: ''
        };
    }

    function createComponent() {
        const index = task.value.ComponentList.length;

        return {
            TaskReference: '',
            Name: '',
            Description: '',
            ComponentType: '',
            Value: '',
            DynamicValue: null,
            ValueDTO: null,
            ValueType: '',
            ValueFormat: '',
            _uid: Number(Date.now()) + index
        };
    }

    async function addStepClick() {
        task.value.StepList.push(createStep());
        updateNumber();
    }

    async function deleteStepClick(index) {
        task.value.StepList.splice(index, 1);
        updateNumber();
    }

    async function addComponentClick() {
        task.value.ComponentList.push(createComponent());
    }

    async function deleteComponentClick(index) {
        task.value.ComponentList.splice(index, 1);
    }

    async function stepFormatValueClick(step) {
        stepValueRef.value = step;
        stepOutputResultRef.value = null;
        componentValueRef.value = null;
        
        const stepList = task.value.StepList.map(item => ({
            Number: item.Number,
            Variable: item.Variable
        }));

        const taskInfo = {
            StepList: stepList
        }
        
        const variableList = utils.retrieveStepVariableList(step.Number, taskInfo, toRaw(store.data.ProcessData));
        const componentInfo = toRaw(task.value.ComponentList.find(f => f.Name == step.Component));

        const buildInputFormatData = {
            Page: "InputPage.html",
            ItemCode: String(step.Number),
            Value: toBase64(step.Value),
            ValueAction: "STEP",
            Component: step.Component,
            Description: step.Description,
            VariableList: variableList,
            Language: store.getLanguage(),
            FormatFile: "InputFields.html",
            RootComponentFolder: "",
            EnabledOptions: true,
            ComponentData: componentInfo,
        };

        window.electronAPI.openPopup("Pages/InputPage.html", buildInputFormatData);
    }

    async function outputResultClick(step) {
        stepValueRef.value = null;
        stepOutputResultRef.value = step;
        componentValueRef.value = null;

        const token = "";
        let outputValue = step.OutputResultOption;
        if (outputValue == null || outputValue == "") {
            outputValue = "{}";
        }

        const outputValueList = JSON.stringify(await adapter.retrieveOutputValueOption(outputValue));

        const data = {
            Page: "OutputValuePage.html",
            ItemCode: String(step.Number),
            Value: outputValueList,
            Component: step.Component,
            Description: step.Description,
            Token: token,
            Language: store.getLanguage(),
            EnabledOptions: true,
        };

        window.electronAPI.openPopup("Pages/OutputValuePage.html", data);
    }

    async function componentFormatValueClick(component) {
        stepValueRef.value = null;
        stepOutputResultRef.value = null;
        componentValueRef.value = component;

        const componentInfo = {
            "ComponentType": component.ComponentType
        };

        const buildInputFormatData = {
            Page: "InputPage.html",
            ItemCode: component.Name,
            Value: toBase64(component.Value),
            ValueAction: "COMPONENT",
            Component: component.Name,
            Description: component.Description,
            VariableList: null,
            Language: store.getLanguage(),
            FormatFile: "InputFields.html",
            RootComponentFolder: "",
            EnabledOptions: false,
            ComponentData: componentInfo,
        };

        window.electronAPI.openPopup("Pages/InputPage.html", buildInputFormatData);
    }

    function onDragStart(index) {
        draggedIndex = index
    }

    function onDragEnd() {
        draggedIndex = null
    }

    function onDrop(targetIndex) {
        if (draggedIndex === null || draggedIndex === targetIndex) return

        const list = task.value.StepList
        const [movedItem] = list.splice(draggedIndex, 1)

        const insertIndex = draggedIndex < targetIndex ? targetIndex : targetIndex
        list.splice(insertIndex, 0, movedItem)

        draggedIndex = null
        updateNumber();
    }

    function tabStepClick() {
        generateComponentTypeList();
        syncStepComponentNamesByUID();
    }

    function tabComponentClick() {
    }

    function tabDeclarationClick() {
        generateComponentTypeList();
    }

    function generateComponentTypeList() {
        componentTypeList.value = task.value.ComponentList
            .filter((component) => component.Name?.trim())
            .map((component) => ({
                Text: component.Name,
                Value: component._uid
            }))
            .sort((a, b) => a.Text.localeCompare(b.Text, 'es'));
    }

    async function updateTaskData() {
        updateNumber();

        if (taskNumber > 0) {
            await store.saveLocalData();
        }
    }

    function goToProcessPage(taskNumber) {
        router.push({
            path: '/Process'
        })
    }

    async function btnAcceptClick() {
        generateComponentTypeList();
        syncStepComponentNamesByUID();
        await updateTaskData();
        goToProcessPage();
    }

    async function btnCancelClick() {
        if (originalTask) {
            Object.assign(task.value, originalTask);
        }
        else {
            store.data.ProcessData.TaskList.pop();            
        }
        store.saveLocalData();
        goToProcessPage();
    }
    
    window.electronAPI.onPopupResult((result) => {
        if (result && result.Result)
        {
            const isTextField = result.Result.hasOwnProperty('isTextField');
            const resultValue = isTextField ? result.Result.value : result.Result;

            if (stepValueRef.value)
            {
                stepValueRef.value.Value = resultValue;
            }
            else if (componentValueRef.value)
            {
                componentValueRef.value.Value = resultValue;
            }
            else if (stepOutputResultRef.value)
            {
                stepOutputResultRef.value.OutputResultOption = result.Result;
            }
        }
    });

    return {
        task,
        taskTitle,
        btnAddStepTop,
        btnAddStepBottom,
        lblTask,
        lblDelay,
        lblNumber,
        lblEnabled,
        btnAccept,
        btnCancel,
        tabStep,
        tabComponent,
        tabDeclaration,
        btnAddComponentTop,
        btnAddComponentBottom,
        componentHeader,
        descriptionHeader,
        encapsulatedHeader,
        variableHeader,
        publicHeader,
        valueHeader,
        valueFormatHeader,
        applyFormulaHeader,
        valueInputTypeHeader,
        valueOutputTypeHeader,
        outputValueOptionHeader,
        declarationTypeHeader,
        delayHeader,
        requiredHeader,
        asyncHeader,
        logHeader,
        disableHeader,
        deleteHeader,
        componentTypeHeaderComponent,
        componentHeaderComponent,
        descriptionHeaderComponent,
        valueHeaderComponent,
        valueFormatComponent,
        valueInputTypeHeaderComponent,
        deleteHeaderComponent,
        editLabel,
        deleteLabel,
        componentList,
        declarationTypeList,
        valueTypeList,
        outputValueList,
        encapsulatedPartitionList,
        componentTypeList,
        btnAcceptClick,
        btnCancelClick,
        addStepClick,
        deleteStepClick,
        addComponentClick,
        deleteComponentClick,
        stepFormatValueClick,
        outputResultClick,
        componentFormatValueClick,
        onComponentChange,
        tabStepClick,
        tabComponentClick,
        tabDeclarationClick,
        onDragStart,
        onDragEnd,
        onDrop
    };

}