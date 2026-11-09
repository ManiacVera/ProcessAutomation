export async function setDefaultSettings(settings) {
  return await window.api.setDefaultSettings(settings);
}

export async function setWorkSpaceDirectory(directory) {
  return await window.api.setWorkSpaceDirectory(directory);
}

export async function retrieveLanguageList() {
  return await window.api.retrieveLanguageList();
}

export async function reloadList() {
  return await window.api.reloadList();
}

export async function retrieveProcessViewerList() {
  return await window.api.retrieveProcessViewerList();
}

export async function regenerateConfigFile() {
  return await window.api.regenerateConfigFile();
}

export async function isRunning() {
  return await window.api.isRunning();
}

export async function stopProgram() {
  if (await isRunning()) {
    return await window.api.stopProgram();
  }
}

export async function saveConfig(data) {
  return await window.api.saveConfig(data);
}

export async function saveProcessItem(data) {
  return await window.api.saveProcessItem(data);
}

export async function copyProcessByNumber(data) {
  return await window.api.copyProcessByNumber(data);
}

export async function deleteProcessByNumber(data) {
  return await window.api.deleteProcessByNumber(data);
}

export async function copyProcessItem(data) {
  return await window.api.copyProcessItem(data);
}

export async function createProcessShortcutByNumber(data) {
  return await window.api.createProcessShortcutByNumber(data);
}

export async function runProcess(data) {
  return await window.api.runProcess(data);
}

export async function runProcessByNumber(data) {
  return await window.api.runProcessByNumber(data);
}

export async function runProcessList(data) {
  if (!await isRunning()) {
    return await window.api.runProcessList(data);
  }
  return null;
}

export async function getProgress() {
  return await window.api.getProgress();
}

export async function getMaximumProgress() {
  return await window.api.getMaximumProgress();
}

export async function getLog() {
  return await window.api.getLog();
}

export async function readProcessFile(processFile) {
  return await window.api.readProcessFile(processFile);
}

export async function saveProcessConfig(data) {
  return await window.api.saveProcessConfig(data);
}

export async function loadUserSettings() {
  return await window.api.loadUserSettings();
}

export async function saveUserSettings(data) {
  return await window.api.saveUserSettings(data);
}