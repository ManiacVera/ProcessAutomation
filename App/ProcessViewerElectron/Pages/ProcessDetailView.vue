<template>
  <div class="col-md-12">
    <fieldset>
      <legend id="processTitle">{{ processTitleLabel }}</legend>
      <div class="row">
        <div class="col-md-2">
          <label id="lblProcessName" for="processName" style="width: 100%;">{{processNameLabel}}:</label>
        </div>
        <div class="col-md-6">
          <input id="txtProcessName" type="text" :value="processName" readonly class="form-control"/>
        </div>
        <div class="col-auto d-flex align-items-end">
          <button id="btnAccept" @click="acceptClick">✅ {{acceptLabel}}</button>
          <button id="btnCancel" @click="goBackClick">❌ {{cancelLabel}}</button>
        </div>
      </div>
    </fieldset>
        
    <fieldset>
        <legend id="processList">{{processListLabel}}</legend>
          <div class="table-responsive table-scroll">
            <table id="tblRunProcessList" class="table table-dark table-bordered table-striped table-sm table-header-freeze">
              <thead>
                <tr>
                  <th >#</th>
                  <th id="taskNameHeader">{{taskNameHeaderLabel}}</th>
                  <th id="stepDescriptionHeader">{{stepDescriptionHeaderLabel}}</th>
                  <th id="inputValueFormatHeader">{{inputValueFormatHeaderLabel}}</th>
                  <th id="valueFormatHeader">{{valueFormatHeaderLabel}}</th>
                  <th id="outputValueTypeHeader">{{outputValueTypeHeaderLabel}}</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(process, index) in processList" :key="process.id || index">
                  <td>{{ index + 1}}</td>
                  <td>{{ process.TaskName }}</td>
                  <td>{{ process.StepDescription }}</td>
                  <td><textarea style="width: 150px; height: 60px;" v-model="process.InputValue"></textarea></td>
                  <td>
                    <button style="width: 20px; padding: 2px;" @click="showInputFormat(process)">...</button>
                  </td>
                  <td>{{ process.OutputValueType }}</td>
                </tr>
              </tbody>
            </table>
          </div>
      </fieldset>
    </div>
</template>

<script setup>

import { pageSetup } from "../Scripts/Pages/ProcessDetailView";

const {
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
    } = pageSetup();

</script>