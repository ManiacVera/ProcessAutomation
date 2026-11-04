// ipc.js
const { ipcMain, dialog, shell } = require('electron');
const path = require('path');
const initI18n = require(path.join(__dirname, 'i18n'));
const { showConfirm, showAlert, openPopup } = require(path.join(__dirname, 'windows.js'));

let i18n;

function setupIpc(mainWindow) {

  ipcMain.on('window-action', (event, action) => {
    if (action === 'minimize') {
      mainWindow.minimize();
    }
    if (action === 'maximize') {
      if (mainWindow.isMaximized()) {
        mainWindow.unmaximize();
      } else {
        mainWindow.maximize();
      }
    }
    if (action === 'fullscreen') {
      if (process.platform === 'darwin') {
        mainWindow.setFullScreen(!mainWindow.isFullScreen());
      }
    }
    if (action === 'close') {
      mainWindow.close();
    }
  });

  ipcMain.on('send-popup-result', (event, result) => {
    mainWindow.webContents.send('on-popup-result', result);
  });

  ipcMain.handle('set-global', (event, key, value) => {
    global.globalData[key] = value;
  });

  ipcMain.handle('get-global', (event, key) => {
    return global.globalData[key];
  });

  ipcMain.handle("show-confirm", async (_, message, title) => {
    return await showConfirm(message, title);
  });

  ipcMain.handle("show-alert", async (_, message, title) => {
    return await showAlert(message, title);
  });

  ipcMain.handle('go-process-page', () => {
    mainWindow.loadFile(path.join(__dirname, 'Pages', 'ProcessPage.html'));
  });

  ipcMain.handle('go-run-process-page', () => {
    mainWindow.loadFile(path.join(__dirname, 'Pages', 'RunProcessPage.html'));
  });

  ipcMain.handle('go-task-page', (event, id) => {
    mainWindow.loadFile(path.join(__dirname, 'Pages', 'TaskPage.html'), {
      query: { taskNumber: id }
    });
  });

  ipcMain.on('open-popup', (event, page, data) => {
    openPopup(mainWindow, page, data);
  });

  ipcMain.handle('open-path', async (event, path) => {
    return shell.openPath(path);
  });
  
  ipcMain.handle('open-process-file', async () => {
    const resultado = await dialog.showOpenDialog({
      properties: ['openFile'],
      filters: [{ name: 'Json Files', extensions: ['json'] }]
    });

    if (resultado.canceled) return null;

    const filePath = resultado.filePaths[0];
    const folderPath = path.dirname(filePath);
    const fileName = path.basename(filePath);

    return { fileName, folderPath, filePath };
  });

  ipcMain.handle('i18n:init', (event, page, lang) => {
    try {
      i18n = initI18n(page, lang);
    } catch (err) {
      console.error('Error en i18n:init:', err);
      return false;
    }
    return true;
  });

  ipcMain.handle('i18n:t', (event, key) => {
    return i18n.t(key);
  });
}

module.exports = setupIpc;