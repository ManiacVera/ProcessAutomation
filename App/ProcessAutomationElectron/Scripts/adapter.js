// --- PROCESS ---
export async function openProcess(processFile, userCode, directory) {
  return await window.api.openProcess(processFile, userCode, directory);
}

export async function saveProcess(data) {
  return await window.api.saveProcess(data);
}

export async function buildProcess(data) {
  return await window.api.buildProcess(data);
}

export async function retrieveProcessList(userCode, directory) {
  return await window.api.retrieveProcessList(userCode, directory);
}

export async function retrieveComponent() {
  return await window.api.retrieveComponent();
}

export async function retrieveDeclarationType() {
  return await window.api.retrieveDeclarationType();
}

export async function retrieveValueAccess() {
  return await window.api.retrieveValueAccess();
}

export async function retrieveValueType() {
  return await window.api.retrieveValueType();
}

export async function retrieveValueOutputType() {
  return await window.api.retrieveValueOutputType();
}

export async function retrieveEncapsulatedPartitionType() {
  return await window.api.retrieveEncapsulatedPartitionType();
}

export async function retrieveOutputValueOption(data) {
  return await window.api.retrieveOutputValueOption(data);
}

export async function retrieveLanguageList() {
  return await window.api.retrieveLanguageList();
}

export async function buildInputFormat(buildDataInfo) {
  return await window.api.buildInputFormat(buildDataInfo);
}

export async function loadUserSettings() {
  return await window.api.loadUserSettings();
}

export async function saveUserSettings(data) {
  return await window.api.saveUserSettings(data);
}

export async function getDataPath() {
  return await window.api.getDataPath();
}


// --- RUN PROCESS ---
export async function setDefaultSettings(settings) {
  return await window.api.setDefaultSettings(settings);
}

export async function initProcess(processData) {
  return await window.api.initProcess(processData);
}

export async function getStepDescription() {
  return await window.api.getStepDescription();
}

export async function getProgress() {
  return await window.api.getProgress();
}

export async function getMaximumProgress() {
  return await window.api.getMaximumProgress();
}

export async function isRunning() {
  return await window.api.isRunning();
}

export async function abortProcess() {
  if (await isRunning()) {
    return await window.api.abortProcess();
  }
}

export async function setInputValue(data) {
  return await window.api.setInputValue(data);
}

export async function runProcess() {
  if (!await isRunning()) {
    return await window.api.runProcess();
  } else {
    console.warn("Process is running");
  }
}