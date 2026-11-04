const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('api', {
  setDefaultSettings: (settings) => ipcRenderer.invoke('api:setDefaultSettings', settings),
  setWorkSpaceDirectory: (directory) => ipcRenderer.invoke('api:setWorkSpaceDirectory', directory),
  retrieveLanguageList: () => ipcRenderer.invoke('api:retrieveLanguageList'),
  reloadList: () => ipcRenderer.invoke('api:reloadList'),
  retrieveProcessViewerList: () => ipcRenderer.invoke('api:retrieveProcessViewerList'),
  regenerateConfigFile: () => ipcRenderer.invoke('api:regenerateConfigFile'),
  isRunning: () => ipcRenderer.invoke('api:isRunning'),
  stopProgram: () => ipcRenderer.invoke('api:stopProgram'),
  saveConfig: (data) => ipcRenderer.invoke('api:saveConfig', data),
  copyProcessByNumber: (data) => ipcRenderer.invoke('api:copyProcessByNumber', data),
  createProcessShortcutByNumber: (data) => ipcRenderer.invoke('api:createProcessShortcutByNumber', data),
  deleteProcessByNumber: (data) => ipcRenderer.invoke('api:deleteProcessByNumber', data),
  saveProcessItem: (data) => ipcRenderer.invoke('api:saveProcessItem', data),
  runProcessList: (data) => ipcRenderer.invoke('api:runProcessList', data),
  getProgress: () => ipcRenderer.invoke('api:getProgress'),
  getMaximumProgress: () => ipcRenderer.invoke('api:getMaximumProgress'),
  getLog: () => ipcRenderer.invoke('api:getLog'),
  readProcessFile: (processFile) => ipcRenderer.invoke('api:readProcessFile', processFile),
  saveProcessConfig: (data) => ipcRenderer.invoke('api:saveProcessConfig', data),
  loadUserSettings: () => ipcRenderer.invoke('api:loadUserSettings'),
  saveUserSettings: (data) => ipcRenderer.invoke('api:saveUserSettings', data),
  buildInputFormat: (buildDataInfo) => ipcRenderer.invoke('api:buildInputFormat', buildDataInfo),
  buildResultValueFormat: (buildDataInfo) => ipcRenderer.invoke('api:buildResultValueFormat', buildDataInfo)
});

contextBridge.exposeInMainWorld("electronAPI", {
  windowAction: (action) => ipcRenderer.send('window-action', action),
  openJsonFile: () => ipcRenderer.invoke('open-json-file'),
  getBaseDirectory: () => ipcRenderer.invoke('get-base-directory'),
  setGlobal: (key, value) => ipcRenderer.invoke('set-global', key, value),
  getGlobal: (key) => ipcRenderer.invoke('get-global', key),
  openPopup: (page, data) => ipcRenderer.send('open-popup', page, data),
  onPopupData: (callback) => ipcRenderer.on('on-popup-data', (_, data) => callback(data)),
  sendPopupResult: (result) => ipcRenderer.send('send-popup-result', result),
  onPopupResult: (callback) => ipcRenderer.on('on-popup-result', (_, data) => callback(data)),
  i18nInit: (page, lang) => ipcRenderer.invoke('i18n:init', page, lang),
  i18nT: (key) => ipcRenderer.invoke('i18n:t', key),
  showAlert: (message, title) => ipcRenderer.invoke("show-alert", message, title),
  showConfirm: (message, title) => ipcRenderer.invoke("show-confirm", message, title),
  onDialogInit: (callback) => ipcRenderer.on('dialog-init', (event, data) => callback(data)),
  sendDialogResult: (value) => ipcRenderer.send('dialog-result', value),
  onLogData: (callback) => { ipcRenderer.on('log-data', (event, data) => { callback(data); }) }
});