<template>
  <div class="col-md-12">
    <fieldset>
      <legend id="runProcessTitle">{{runProcessTitle}}</legend>
      <br />
      <div class="row g-3">
        <div class="col-md-7">
          <label id="lblProcessName" for="txtProcess">{{processNameLabel}}</label>
          <input type="text" id="txtProcess" class="form-control" :value="model.ProcessData.ProcessName" readonly/>
        </div>
        <div class="col-md-3">
          <br />
          <div class="progress-wrapper">
            <progress class="progress-bar" id="progressBar" :max="maxProgress" :value="progress"></progress>
            <span class="progress-text" id="progressText">{{ progressText }}</span>
          </div>
        </div>
      </div>
      
      <div class="mt-3 text-end">
        <button id="btnRunProcess" style="width: 110px;" @click="runProcessClick">▶️ {{btnRunProcessLabel}}</button>
        <button id="btnAbortProcess" style="width: 110px;" @click="abortProcessClick">🚫 {{btnAbortProcessLabel}}</button>
        <button id="btnBack" style="width: 110px;" @click="backClick">⬅️ {{btnBackLabel}}</button>
      </div>
    </fieldset>

    <fieldset>
      <legend id="RunProcessList">{{runProcessListLabel}}</legend>
        <div class="table-responsive table-scroll">
          <table id="tblRunProcessList" class="table table-dark table-bordered table-striped table-sm table-header-freeze">
            <thead>
              <tr>
                <th >#</th>
                <th id="taskHeader">{{taskHeader}}</th>
                <th id="descriptionHeader">{{descriptionHeader}}</th>
                <th id="valueHeader">{{valueHeader}}</th>
                <th id="valueFormatHeader">{{valueFormatHeader}}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(runProcess, index) in runProcessList.filter(x => x.ValueTypeDeclaration === 'EDITABLE')" :key="index">
                <td>
                  <span class="step-index-runprocess">{{index}}</span>
                </td>
                <td>
                  <input type="text" class="task-runprocess" style="width: 200px;" :value="runProcess.TaskName" readonly/>
                </td>
                <td>
                  <input type="text" class="description-runprocess" style="width: 200px;" :value="runProcess.StepDescription" readonly/>
                </td>
                <td>
                  <textarea class="value-runprocess" style="width: 150px; height: 60px;" v-model="runProcess.InputValue"></textarea>
                </td>
                <td>
                  <button class="btn-value-format-runprocess" style="width: 20px; padding: 2px;" @click="stepFormatValueClick(runProcess)">...</button>
                </td>
            </tr>
            </tbody>
          </table>
        </div>
    </fieldset>
  </div>
</template>

<script setup>
import { pageSetup } from "../Scripts/Pages/RunProcessView";

const
{
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
} = pageSetup();

</script>