global.globalData = {};

const { app, BrowserWindow, Menu } = require('electron');
const path = require('path');
const { createServerApi, createServerPipe, generatePipeName } = require(path.join(__dirname, 'servers.js'));
const setupIpc = require(path.join(__dirname, 'ipc.js'));
const setupIpcApi = require(path.join(__dirname, 'ipc-api.js'));

let mainWindow;

function setMenu() {  
  const menuTemplate = [
    {
      label: app.name,
      submenu: [
        { label: 'About', role: 'about' },
        { type: 'separator' },
        { label: 'Quit', role: 'quit' }
      ]
    },
    {
      label: 'Edit',
      submenu: [
        { role: 'undo' },
        { role: 'redo' },
        { type: 'separator' },
        { role: 'cut' },
        { role: 'copy' },
        { role: 'paste' },
        { role: 'selectAll' }
      ]
    }
  ];

  const menu = Menu.buildFromTemplate(menuTemplate);
  Menu.setApplicationMenu(menu);
}

async function createWindow() {
  setMenu();
  await createServerApi();
  const api = setupIpcApi();
  await api.waitForPing();

  global.pipeName = generatePipeName();

  await createServerPipe(mainWindow, global.pipeName);

  mainWindow = new BrowserWindow({
    frame: false,
    backgroundColor: '#212529',
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      nodeIntegration: false,
      contextIsolation: true,
      devTools: process.env.DEBUG_MODE === 'true'
    }
  });

  global.mainWindow = mainWindow;

  if (process.platform === 'darwin') {
    mainWindow.setFullScreenable(true);
  }

  mainWindow.maximize();
  mainWindow.loadFile('dist/index.html');

  if (process.env.DEBUG_MODE === 'true') {
    mainWindow.webContents.on('before-input-event', (event, input) => {
      if (input.control && input.shift && input.key.toLowerCase() === 'i') {
        mainWindow.webContents.openDevTools();
      }
    });
  }
  
  api.setDefaultSettings({ "PipeName": global.pipeName });  

  setupIpc(mainWindow);
}

app.setAppUserModelId("com.processautomation.app");
app.whenReady().then(createWindow);

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});