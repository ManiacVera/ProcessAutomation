<template>
    <div class="col-md-12">

    <fieldset>
        <legend>{{ taskTitle }}</legend>

        <div class="row g-3 align-items-center">
        <div class="col-md-2">
            <label>{{ lblNumber }}</label>
            <input id="txtNumber" type="number" class="form-control" v-model.number="task.Number">
        </div>

        <div class="col-md-6">
            <label>{{ lblTask }}</label>
            <input id="txtTask" type="text" class="form-control" v-model="task.Name">
        </div>

        <div class="col-md-2">
            <label>{{ lblDelay }}</label>
            <input id="txtDelay" type="number" class="form-control" v-model="task.Delay">
        </div>

        <div class="col-md-2 d-flex align-items-end">
            <div class="form-check">
            <input id="chkEnabled" class="form-check-input" type="checkbox" v-model="task.Enabled">
            <label class="form-check-label">{{ lblEnabled }}</label>
            </div>
        </div>
        </div>

        <div class="mt-3 text-end">
        <button @click="btnAcceptClick">✅ {{ btnAccept }}</button>
        <button @click="btnCancelClick">❌ {{ btnCancel }}</button>
        </div>
    </fieldset>

    <fieldset>
        <legend>Lista de Pasos</legend>

        <!-- Tabs -->
        <ul class="nav nav-tabs">
        <li class="nav-item">
            <button class="nav-link active" data-bs-toggle="tab" data-bs-target="#pasos" @click="tabStepClick">
            {{ tabStep }}
            </button>
        </li>
        <li class="nav-item">
            <button class="nav-link" data-bs-toggle="tab" data-bs-target="#componentes" @click="tabComponentClick">
            {{ tabComponent }}
            </button>
        </li>
        </ul>

        <div class="tab-content mt-3">
        <div class="tab-pane fade show active" id="pasos">
            <div class="d-flex justify-content-between mb-2">
                <button class="add-btn-top" @click="addStepClick">➕ {{ btnAddStepTop }}</button>
            </div>

            <div class="table-responsive table-scroll">
                <table class="table table-dark table-bordered table-striped table-sm table-header-freeze">
                    <thead>
                    <tr>
                        <th></th>
                        <th>#</th>
                        <th>{{ componentHeader }}</th>
                        <th>{{ descriptionHeader }}</th>
                        <th>{{ encapsulatedHeader }}</th>
                        <th>{{ variableHeader }}</th>
                        <th>{{ publicHeader }}</th>
                        <th>{{ valueHeader }}</th>
                        <th>{{ valueFormatHeader }}</th>
                        <th>{{ applyFormulaHeader }}</th>
                        <th>{{ valueInputTypeHeader }}</th>
                        <th>{{ valueOutputTypeHeader }}</th>
                        <th>{{ outputValueOptionHeader }}</th>
                        <th>{{ declarationTypeHeader }}</th>
                        <th>{{ delayHeader }}</th>
                        <th>{{ requiredHeader }}</th>
                        <th>{{ asyncHeader }}</th>
                        <th>{{ logHeader }}</th>
                        <th>{{ disableHeader }}</th>
                        <th>{{ deleteHeader }}</th>
                    </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(step, index) in task.StepList" :key="index"
                            draggable="false" 
                            @dragover.prevent
                            @drop="onDrop(index)">

                            <td class="drag-handle" draggable="true" @dragstart="onDragStart(index)"
                                @dragend="onDragEnd" style="cursor: grab;">
                            ⇅
                            </td>
                            <td>
                                <input type="number" v-model="step.Number" style="width: 60px;">
                            </td>
                            <td>
                                <select
                                    v-model="step._componentSelected"
                                    @change="onComponentChange($event, step)">
                                    <option v-for="item in componentTypeList" :key="item.Value" :value="item.Value">
                                        {{ item.Text }}
                                    </option>
                                </select>
                            </td>
                            <td>
                                <input type="text" v-model="step.Description" style="width: 200px;">
                            </td>
                            <td>
                                <select v-model="step.EncapsulatedPartitionType">
                                    <option v-for="item in encapsulatedPartitionList" :key="item.Text" :value="item.Text">
                                        {{ item.Text }}
                                    </option>
                                </select>
                            </td>
                            <td>
                                <input type="text" v-model="step.Variable">
                            </td>                        
                            <td class="text-center">
                                <input type="checkbox" v-model="step.IsPublicVariable">
                            </td>
                            <td>
                                <textarea v-model="step.Value" style="width: 150px; height: 60px;"></textarea>
                            </td>
                            <td>
                                <button class="btn-value-format-task" style="width: 24px; padding: 2px;" @click="stepFormatValueClick(step)">📝</button>
                            </td>
                            <td class="text-center">
                                <input type="checkbox" v-model="step.ApplyFormula">
                            </td>
                            <td>
                                <select v-model="step.InputValueType">
                                    <option v-for="item in valueTypeList" :key="item.Text" :value="item.Text">
                                        {{ item.Text }}
                                    </option>
                                </select>
                            </td>
                            <td>
                                <select v-model="step.OutputValueType">
                                    <option v-for="item in outputValueList" :key="item.Text" :value="item.Text">
                                        {{ item.Text }}
                                    </option>
                                </select>
                            </td>
                            <td>
                                <button class="btn-output-result-option-task" style="width: 24px; padding: 2px;" @click="outputResultClick(step)">📝</button>
                            </td>
                            <td>
                                <select v-model="step.ValueTypeDeclaration">
                                    <option v-for="item in declarationTypeList" :key="item.Text" :value="item.Text">
                                        {{ item.Text }}
                                    </option>
                                </select>
                            </td>
                            <td>
                                <input type="number" v-model="step.Delay">
                            </td>
                            <td class="text-center">
                                <input type="checkbox" v-model="step.Required">
                            </td>
                            <td class="text-center">
                                <input type="checkbox" v-model="step.Async">
                            </td>
                            <td class="text-center">
                                <input type="checkbox" v-model="step.Log">
                            </td>
                            <td class="text-center">
                                <input type="checkbox" v-model="step.Disable">
                            </td>
                            <td class="text-center">
                                <button class="btn btn-danger btn-sm" style="width: 24px; padding: 2px;" @click="deleteStepClick(index)">❌</button>
                            </td>
                        </tr>
                        </tbody>
                </table>
            </div>

            <div class="d-flex justify-content-between mb-2">
                <button class="add-btn-top" @click="addStepClick">➕ {{ btnAddStepBottom }}</button>
            </div>
        </div>

        <div class="tab-pane fade" id="componentes">
            <div class="d-flex justify-content-between mb-2">
                <button class="add-btn-top" @click="addComponentClick">➕ {{ btnAddComponentTop }}</button>
            </div>

            <div class="table-responsive table-scroll">
                <table class="table table-dark table-bordered table-striped table-sm">
                    <thead>
                    <tr>
                        <th>{{ componentTypeHeaderComponent }}</th>
                        <th>{{ componentHeaderComponent }}</th>
                        <th>{{ descriptionHeaderComponent }}</th>
                        <th>{{ valueHeaderComponent }}</th>
                        <th>{{ valueFormatComponent }}</th>
                        <th>{{ valueInputTypeHeaderComponent }}</th>
                        <th>{{ deleteHeaderComponent }}</th>
                    </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(component, index) in task.ComponentList" :key="index">
                            <td>
                                <select v-model="component.ComponentType">
                                    <option v-for="item in componentList" :key="item.Value" :value="item.Value">
                                        {{ item.Text }}
                                    </option>
                                </select>
                            </td>
                            <td>
                                <input type="text" v-model="component.Name" />
                            </td>
                            <td>
                                <input type="text" v-model="component.Description" />
                            </td>
                            <td>
                                <textarea v-model="component.Value"></textarea>
                            </td>
                            <td class="text-center"> 
                                <button style="width: 24px; padding: 2px;" @click="componentFormatValueClick(component)">📝</button>
                            </td>
                            <td>
                                <select v-model="component.ValueType">
                                    <option v-for="item in valueTypeList" :key="item.Text" :value="item.Text">
                                        {{ item.Text }}
                                    </option>
                                </select>
                            </td>
                            <td class="text-center">
                                <button style="width: 24px; padding: 2px;" @click="deleteComponentClick(index)">❌</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="d-flex justify-content-between mb-2">
                <button class="add-btn-top" @click="addComponentClick">➕ {{ btnAddComponentBottom }}</button>
            </div>
        </div>

        <div class="tab-pane fade" id="declaracion">
            <textarea class="form-control" rows="5"></textarea>
        </div>

        </div>
    </fieldset>
    </div>
</template>
<script setup>
import { pageSetup } from "../Scripts/Pages/TaskView";

const  
{
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
} = pageSetup();

</script>