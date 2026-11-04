<template>
  <div class="col-md-12">
    <fieldset>
      <legend>{{fileTitleLabel}}</legend>
      <div class="row g-3">
        <div class="col-md-9">
          <input type="text" class="form-control" style="font-weight: bold;" :value="processViewerFile" readonly />
        </div>
        <div class="col-md-3">
          <button @click="openProcessViewerClick">📂 {{openProcessViewerLabel}}</button>
        </div>
      </div>
    </fieldset>

    <fieldset>
      <legend id="processViewerTitle">{{processViewerTitleLabel}}</legend>
      <div class="row g-3">
        <div class="col-md-7">
          <input type="text" id="txtProcess" class="form-control" style="text-align: center; font-weight: bold;" :value="processName" readonly />
        </div>
        <div class="col-md-3">
          <div class="progress-wrapper">
            <progress class="progress-bar" id="progressBar" :max="maxProgress" :value="progress"></progress>
            <span class="progress-text" id="progressText">{{ stepName }}</span>
          </div>
        </div>
        <button id="btnRun" style="width: 100px;" @click="runClick">▶️ {{runLabel}}</button>
      </div>
    </fieldset>

    <fieldset>
      <legend id="processList">{{processListLabel}}</legend>
        <div class="mt-3">
          <button id="btnSave" style="width: 100px;" @click="saveClick">💾 {{saveLabel}}</button>
          <button id="btnProcessDetail" @click="goToProcessDetail" style="display: none;">{{processDetailLabel}}</button>
          <button id="btnCopy" style="width: 100px;" @click="copyProcessClick">📋 {{copyLabel}}</button>
          <button id="btnCreateShortcut" style="width: 150px;" @click="createProcessShortcutClick">⚡ {{shortcutLabel}}</button>
        </div>
        <br>
        <div class="table-responsive table-scroll">
          <table id="tblProcessList" class="table table-dark table-bordered table-striped table-sm table-header-freeze">
            <thead>
              <tr>
                <th id="selectHeader">
                  <br/>
                  <input type="checkbox" v-model="isAllSelected"/>
                </th>
                <th >#</th>
                <th id="processHeader">{{processHeaderLabel}}</th>
                <th id="folderHeader">{{folderHeaderLabel}}</th>
                <th id="showInputHeader">{{showInputHeaderLabel}}</th>
                <th id="editHeader">{{editHeaderLabel}}</th>
                <th id="deleteHeader">{{deleteHeaderLabel}}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(process, index) in processViewerList" :key="process.id || index">
                <td>
                  <input type="checkbox" :value="process.ProcessNumber" v-model="selectedProcesses"/>
                </td>
                <td>{{ process.ProcessNumber }}</td>
                <td>
                  <span v-if="processEdit.ProcessNumber !== process.ProcessNumber">
                    {{ process.ProcessName }}
                  </span>

                  <input  v-else id="txtProcessName" type="text" v-model="process.ProcessName"/>
                </td>
                <td>{{ process.Folder }}</td>
                <td>
                  <button style="width: 20px; padding: 2px;" @click="goToProcessDetail(process)">...</button>
                </td>
                <td>
                  <div v-if="process.IsCopy">
                    <button v-if="processEdit.ProcessNumber !== process.ProcessNumber" style="width: 20px; padding: 2px; transform: scaleX(-1);" @click="editProcessClick(process)">✏️</button>
                    <button v-else style="width: 20px; padding: 2px;" @click="saveProcessClick(process)">💾</button>
                  </div>
                </td>
                <td>
                  <div v-if="process.IsCopy">
                    <button style="width: 20px; padding: 2px;" @click="deleteProcessClick(process.ProcessNumber)">❌</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
    </fieldset>
  </div>
</template>

<script setup>
import { pageSetup } from "../Scripts/Pages/ProcessListView";

const { 
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
      } = pageSetup();

</script>