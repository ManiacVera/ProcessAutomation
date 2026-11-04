<template>
  <div class="col-md-12">
      <fieldset>
        <legend id="processTitle">{{processTitle}}</legend>
        <br />
        <div class="row g-3">
          <div class="col-md-8">
            <select v-model="model.ProcessFile" :disabled="isProcessFileDisabled" class="form-select process-disabled">
                <option v-for="item in processList" :key="item.Value" :value="item.Value">
                    {{ item.Text }}
                </option>
            </select>
          </div>
          <div class="col-md-4 d-flex gap-2">
            <button id="btnOpen" style="width: 110px;" @click="openClick">📂 {{btnOpen}}</button>
            <button id="btnClose" style="width: 110px;" @click="closeClick">🔒 {{btnClose}}</button>
          </div>
        </div>

        <br />
        <div class="row g-3">
          <div class="col-md-2">
            <label id="lblNumber" for="numero">{{lblNumber}}</label>
            <input type="number" id="txtNumber" value="0" min="0" class="form-control" v-model.number="model.ProcessData.Number" />
          </div>
          <div class="col-md-10">
            <label id="lblProcessName" for="txtProcess">{{lblProcessName}}</label>
            <input type="text" id="txtProcess" class="form-control" v-model="model.ProcessData.ProcessName"/>
          </div>
        </div>
        
        <div class="mt-3 d-flex justify-content-between">
          <div class="d-flex gap-2">
            <button id="btnClean" @click="cleanClick">🗑️ {{btnClean}}</button>
            <button id="btnOpenDataFolder" @click="openDataFolderClick">📁 {{btnOpenDataFolder}}</button>
          </div>

          <div class="d-flex gap-2">
            <button id="btnSave" style="width: 110px;" @click="saveClick">💾 {{btnSave}}</button>
            <button id="btnBuild" style="width: 110px;" @click="buildClick">⚙️ {{btnBuild}}</button>
            <button id="btnImport" style="width: 110px;" @click="importClick">📥 {{btnImport}}</button>
            <button id="btnRun" style="width: 140px;" @click="runClick">▶️ {{btnRun}}</button>
          </div>
        </div>
      </fieldset>

      <fieldset>
        <legend id="taskTitle">{{taskTitle}}</legend>
        <button class="add-btn-top" id="btnAddTaskTop" @click="addTaskClick">📋 {{btnAddTaskTop}}</button>
        <div class="table-responsive table-scroll">
          <table id="tblTaskList" class="table table-dark table-bordered table-striped table-sm">
            <thead>
              <tr>
                <th id="numberHeader">{{numberHeader}}</th>
                <th id="nameHeader">{{nameHeader}}</th>
                <th id="enabledHeader">{{enabledHeader}}</th>
                <th id="editHeader">{{editHeader}}</th>
                <th id="deleteHeader">{{deleteHeader}}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(task, index) in model.ProcessData.TaskList" :key="task.id || index">
                <td>{{ task.Number }}</td>
                <td>{{ task.Name }}</td>
                <td>
                    <input type="checkbox" :checked="task.Enabled" disabled></input>
                </td>
                <td>
                  <button class="btn-edit-task" @click="editTaskClick(task.Number)"><span style="display: inline-block; transform: scaleX(-1);">✏️️</span></button>
                </td>
                <td>
                  <button class="btn-delete-task" @click="deleteTaskClick(task.Number)">❌</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <button class="add-btn-bottom" id="btnAddTaskBottom" @click="addTaskClick">📋 {{btnAddTaskBottom}}</button>
      </fieldset>
    </div>
</template>

<script setup>
import { pageSetup } from "../Scripts/Pages/ProcessView";

const
{
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
    btnImport,
    btnRun,
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
} = pageSetup();
</script>